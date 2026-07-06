using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Text;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace sapservice.Controllers
{

    using myapp.Data;
    using myapp.Models;
    using myapp.Models.ViewModels;
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public ReportController(ApplicationDbContext context, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _context = context;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public IActionResult ExportToPdf(int id)
        {
            var item = _context.RequestItems
                .Include(x => x.Routings)
                .Include(x => x.BomComponents)
                .FirstOrDefault(x => x.Id == id);
            if (item == null)
                return NotFound();

            PopulateReportFields(item);

            // ใช้ QuestPDF สร้าง PDF
            var document = new myapp.Documents.SAPServicePdfDocument(item);
            using var stream = new System.IO.MemoryStream();
            document.GeneratePdf(stream);
            var pdfBytes = stream.ToArray();
            var fileName = $"SAPService_{item.DocumentNumber}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }


        public IActionResult SAPService()
        {
            var items = _context.RequestItems
                .Where(x => x.UsageStatus == 1)
                .OrderByDescending(x => x.RequestDate)
                .ToList();

            foreach (var item in items)
            {
                PopulateReportFields(item);
            }

            return View(items);
        }

        public IActionResult CaseTracking(string? searchTerm, string? requestType, string? status)
        {
            var query = _context.RequestItems
                .Where(x => x.UsageStatus == 1);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(x =>
                    (x.DocumentNumber ?? "").ToLower().Contains(term) ||
                    (x.Requester ?? "").ToLower().Contains(term) ||
                    (x.Description ?? "").ToLower().Contains(term) ||
                    (x.RequestType ?? "").ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(requestType))
            {
                query = query.Where(x => x.RequestType == requestType);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            var requests = query
                .OrderByDescending(x => x.RequestDate)
                .ToList();

            var users = _context.Users.ToList();
            var requestIds = requests.Select(r => r.Id.ToString()).ToList();
            var auditLogs = _context.AuditLogs
                .Where(a => a.EntityName == nameof(RequestItem)
                    && a.EntityId != null
                    && requestIds.Contains(a.EntityId))
                .OrderBy(a => a.PerformedAt)
                .ToList();

            string ResolveUserDisplayName(string? userId)
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return "-";
                }

                var user = users.FirstOrDefault(u => u.Id == userId)
                    ?? users.FirstOrDefault(u => string.Equals(u.UserName, userId, StringComparison.OrdinalIgnoreCase))
                    ?? users.FirstOrDefault(u => string.Equals(u.Email, userId, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    return userId;
                }

                var fullName = $"{user.FirstName} {user.LastName}".Trim();
                return string.IsNullOrWhiteSpace(fullName) ? userId : fullName;
            }

            string? ResolveRequesterDepartment(string? requester)
            {
                if (string.IsNullOrWhiteSpace(requester))
                {
                    return null;
                }

                var requesterUser = users.FirstOrDefault(u =>
                    string.Equals($"{u.FirstName} {u.LastName}".Trim(), requester.Trim(), StringComparison.OrdinalIgnoreCase)
                    || string.Equals(u.UserName, requester.Trim(), StringComparison.OrdinalIgnoreCase)
                    || string.Equals(u.Email, requester.Trim(), StringComparison.OrdinalIgnoreCase));

                return requesterUser?.Department;
            }

            string? ExtractAssignedITUserId(string? details)
            {
                if (string.IsNullOrWhiteSpace(details))
                {
                    return null;
                }

                const string marker = "AssignedITUserId:";
                var markerIndex = details.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (markerIndex < 0)
                {
                    return null;
                }

                var valueStart = markerIndex + marker.Length;
                var rest = details.Substring(valueStart);
                var endIndex = rest.IndexOf(';');
                var segment = (endIndex >= 0 ? rest[..endIndex] : rest).Trim();
                var parts = segment.Split("->", StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    return string.IsNullOrWhiteSpace(parts[1]) ? null : parts[1].Trim();
                }

                return null;
            }

            DateTime? ResolveAssignedAt(RequestItem request, List<AuditLog> requestLogs)
            {
                var matchingLog = requestLogs
                    .Where(log => string.Equals(ExtractAssignedITUserId(log.Details), request.AssignedITUserId, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(log => log.PerformedAt)
                    .FirstOrDefault();

                if (matchingLog != null)
                {
                    return matchingLog.PerformedAt;
                }

                return !string.IsNullOrWhiteSpace(request.AssignedITUserId)
                    ? request.UpdatedAt ?? request.RequestDate
                    : null;
            }

            bool IsClosedStatus(string? status)
            {
                return string.Equals(status, "Complete", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(status, "Approved", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(status, "Rejected", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(status, "Reject", StringComparison.OrdinalIgnoreCase);
            }

            string FormatDuration(TimeSpan duration)
            {
                if (duration.TotalMinutes < 1)
                {
                    return "< 1 minute";
                }

                if (duration.TotalHours < 1)
                {
                    return $"{Math.Round(duration.TotalMinutes)} min";
                }

                var days = (int)duration.TotalDays;
                var hours = duration.Hours;
                var minutes = duration.Minutes;

                if (days > 0)
                {
                    return $"{days}d {hours}h {minutes}m";
                }

                return $"{(int)duration.TotalHours}h {minutes}m";
            }

            var reportItems = requests.Select(request =>
            {
                var requestLogs = auditLogs
                    .Where(a => a.EntityId == request.Id.ToString())
                    .ToList();

                var assignedAt = ResolveAssignedAt(request, requestLogs);
                DateTime? closedAt = IsClosedStatus(request.Status)
                    ? request.UpdatedAt ?? assignedAt ?? request.RequestDate
                    : null;
                var durationEnd = closedAt ?? request.UpdatedAt ?? DateTime.UtcNow;
                var duration = assignedAt.HasValue && durationEnd >= assignedAt.Value
                    ? durationEnd - assignedAt.Value
                    : TimeSpan.Zero;

                return new CaseTrackingReportItemViewModel
                {
                    RequestId = request.Id,
                    DocumentNumber = request.DocumentNumber ?? $"REQ-{request.Id}",
                    RequestType = request.RequestType,
                    Requester = request.Requester,
                    RequesterDepartment = ResolveRequesterDepartment(request.Requester),
                    Status = request.Status,
                    Description = request.Description,
                    AssignedToName = ResolveUserDisplayName(request.AssignedITUserId),
                    RequestDate = request.RequestDate,
                    AssignedAt = assignedAt,
                    LastUpdatedAt = request.UpdatedAt,
                    ClosedAt = closedAt,
                    WorkingHours = Math.Round(duration.TotalHours, 2),
                    WorkingDurationText = assignedAt.HasValue ? FormatDuration(duration) : "-"
                };
            }).ToList();

            ViewBag.RequestTypes = _context.RequestItems
                .Where(x => x.UsageStatus == 1 && x.RequestType != null)
                .Select(x => x.RequestType)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            ViewBag.Statuses = _context.RequestItems
                .Where(x => x.UsageStatus == 1 && x.Status != null)
                .Select(x => x.Status)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return View(reportItems);
        }

        private void PopulateReportFields(RequestItem item)
        {
            if (item == null)
            {
                return;
            }

            var requesterName = (item.Requester ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(requesterName))
            {
                var requesterUser = _context.Users
                    .AsEnumerable()
                    .FirstOrDefault(u =>
                        string.Equals($"{u.FirstName} {u.LastName}".Trim(), requesterName, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(u.UserName, requesterName, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(u.Email, requesterName, StringComparison.OrdinalIgnoreCase));

                item.RequesterEmail = requesterUser?.Email;
                item.RequesterDepartment = requesterUser?.Department;
            }

            var updatedByValue = (item.UpdatedBy ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(updatedByValue))
            {
                var updatedByUser = _context.Users
                    .AsEnumerable()
                    .FirstOrDefault(u =>
                        string.Equals(u.UserName, updatedByValue, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(u.Email, updatedByValue, StringComparison.OrdinalIgnoreCase)
                        || string.Equals($"{u.FirstName} {u.LastName}".Trim(), updatedByValue, StringComparison.OrdinalIgnoreCase));

                item.UpdatedByDisplayName = updatedByUser != null
                    ? $"{updatedByUser.FirstName} {updatedByUser.LastName}".Trim()
                    : item.UpdatedBy;
            }
        }

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            var items = _context.RequestItems
                .Where(x => x.UsageStatus == 1)
                .OrderByDescending(x => x.RequestDate)
                .ToList();

            foreach (var item in items)
            {
                PopulateReportFields(item);
            }

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var ws = workbook.Worksheets.Add("SAP Service Report");

            // Header
            var headers = new[] { "Document No.", "Requester", "Department", "Email", "Request Type", "Status", "Description", "Date" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
                ws.Cell(1, i + 1).Style.Font.Bold = true;
                ws.Cell(1, i + 1).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#FF8C1A");
                ws.Cell(1, i + 1).Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
            }

            // Data rows
            int row = 2;
            foreach (var item in items)
            {
                ws.Cell(row, 1).Value = item.DocumentNumber;
                ws.Cell(row, 2).Value = item.Requester;
                ws.Cell(row, 3).Value = item.RequesterDepartment;
                ws.Cell(row, 4).Value = item.RequesterEmail;
                ws.Cell(row, 5).Value = item.RequestType;
                ws.Cell(row, 6).Value = item.Status;
                ws.Cell(row, 7).Value = item.Description;
                ws.Cell(row, 8).Value = item.RequestDate.ToString("dd/MM/yyyy");
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new System.IO.MemoryStream();
            workbook.SaveAs(stream);
            var bytes = stream.ToArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"SAPServiceReport_{System.DateTime.Now:yyyyMMdd}.xlsx");
        }

// ...existing code...


        // Render Razor view to string for PDF
        private async Task<string> RenderViewAsync(string viewName, object model)
        {
            var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor, ModelState);
            using var sw = new StringWriter();
            var viewResult = _viewEngine.FindView(actionContext, viewName, false);
            if (!viewResult.Success)
                throw new InvalidOperationException($"View '{viewName}' not found.");
            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), ModelState) { Model = model };
            var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary, new TempDataDictionary(HttpContext, _tempDataProvider), sw, new HtmlHelperOptions());
            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
    }
}
