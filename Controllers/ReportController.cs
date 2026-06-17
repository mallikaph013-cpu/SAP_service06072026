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
