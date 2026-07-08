#nullable enable

using ITRepairService.Data;
using ITRepairService.Models;
using ITRepairService.ViewModels.RepairTickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ITRepairService.Controllers;

[Authorize]
public class RepairTicketsController(AppDbContext context, UserManager<ApplicationUser> userManager) : Controller
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<IActionResult> Index(string? status, string? sort, string? dir)
    {
        var query = _context.RepairTickets.AsNoTracking().AsQueryable();
        var canSeeInProgress = User.IsInRole(AppRoles.Admin) || User.IsInRole(AppRoles.ITSupport);

        var currentUser = await _userManager.GetUserAsync(User);
        var currentUserId = currentUser?.Id;
        var currentUserFullName = currentUser?.FullName?.Trim();
        var currentUserUserName = currentUser?.UserName?.Trim();
        var currentUserEmail = currentUser?.Email?.Trim();

        var isAdmin = User.IsInRole(AppRoles.Admin);
        var isApprove = User.IsInRole(AppRoles.Approve);

        if (!isAdmin)
        {
            if (isApprove)
            {
                query = query.Where(ticket =>
                    (!string.IsNullOrWhiteSpace(currentUserId) && ticket.ApproverUserId == currentUserId)
                    || (!string.IsNullOrWhiteSpace(currentUserFullName) && ticket.ApproverName == currentUserFullName)
                    || (!string.IsNullOrWhiteSpace(currentUserUserName) && ticket.ApproverName == currentUserUserName)
                    || (!string.IsNullOrWhiteSpace(currentUserEmail) && ticket.ApproverName == currentUserEmail)
                    || (!string.IsNullOrWhiteSpace(currentUserId) && ticket.RequesterUserId == currentUserId)
                    || (string.IsNullOrWhiteSpace(ticket.RequesterUserId)
                        && (
                            (!string.IsNullOrWhiteSpace(currentUserFullName) && ticket.RequesterName == currentUserFullName)
                            || (!string.IsNullOrWhiteSpace(currentUserUserName) && ticket.RequesterName == currentUserUserName)
                            || (!string.IsNullOrWhiteSpace(currentUserEmail) && ticket.RequesterName == currentUserEmail)
                            || (!string.IsNullOrWhiteSpace(currentUserFullName) && ticket.CreatedByName == currentUserFullName)
                            || (!string.IsNullOrWhiteSpace(currentUserUserName) && ticket.CreatedByName == currentUserUserName)
                            || (!string.IsNullOrWhiteSpace(currentUserEmail) && ticket.CreatedByName == currentUserEmail)
                        )));
            }
            else
            {
                query = query.Where(ticket =>
                    (!string.IsNullOrWhiteSpace(currentUserId) && ticket.RequesterUserId == currentUserId)
                    || (string.IsNullOrWhiteSpace(ticket.RequesterUserId)
                        && (
                            (!string.IsNullOrWhiteSpace(currentUserFullName) && ticket.RequesterName == currentUserFullName)
                            || (!string.IsNullOrWhiteSpace(currentUserUserName) && ticket.RequesterName == currentUserUserName)
                            || (!string.IsNullOrWhiteSpace(currentUserEmail) && ticket.RequesterName == currentUserEmail)
                            || (!string.IsNullOrWhiteSpace(currentUserFullName) && ticket.CreatedByName == currentUserFullName)
                            || (!string.IsNullOrWhiteSpace(currentUserUserName) && ticket.CreatedByName == currentUserUserName)
                            || (!string.IsNullOrWhiteSpace(currentUserEmail) && ticket.CreatedByName == currentUserEmail)
                        )));
            }
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TicketStatus>(status, true, out var parsedStatus))
        {
            if (parsedStatus != TicketStatus.InProgress || canSeeInProgress)
            {
                query = query.Where(ticket => ticket.Status == parsedStatus);
                ViewData["CurrentStatus"] = parsedStatus.ToString();
            }
        }
        else
        {
            // No status filter selected: show all statuses, including Closed.
        }

        var currentSort = string.IsNullOrWhiteSpace(sort) ? "created" : sort.Trim().ToLowerInvariant();
        var currentDir = string.Equals(dir, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        query = (currentSort, currentDir) switch
        {
            ("requester", "asc") => query.OrderBy(ticket => ticket.RequesterName).ThenByDescending(ticket => ticket.CreatedAt),
            ("requester", "desc") => query.OrderByDescending(ticket => ticket.RequesterName).ThenByDescending(ticket => ticket.CreatedAt),
            ("priority", "asc") => query.OrderBy(ticket => ticket.Priority).ThenByDescending(ticket => ticket.CreatedAt),
            ("priority", "desc") => query.OrderByDescending(ticket => ticket.Priority).ThenByDescending(ticket => ticket.CreatedAt),
            ("created", "asc") => query.OrderBy(ticket => ticket.CreatedAt),
            _ => query.OrderByDescending(ticket => ticket.CreatedAt)
        };

        ViewData["CurrentSort"] = currentSort;
        ViewData["CurrentDir"] = currentDir;

        ViewData["CurrentUserId"] = currentUserId;
        ViewData["CurrentUserFullName"] = currentUserFullName;
        ViewData["CurrentUserUserName"] = currentUserUserName;
        ViewData["CurrentUserEmail"] = currentUserEmail;

        var tickets = await query.ToListAsync();

        var latestStatusUpdatedBy = new Dictionary<int, string>();
        if (tickets.Count > 0)
        {
            var ticketIds = tickets.Select(ticket => ticket.Id).ToList();
            var timeline = await _context.RepairTicketStatusHistories
                .AsNoTracking()
                .Where(history => ticketIds.Contains(history.RepairTicketId))
                .OrderByDescending(history => history.ChangedAt)
                .ThenByDescending(history => history.Id)
                .Select(history => new { history.RepairTicketId, history.ChangedByName })
                .ToListAsync();

            foreach (var item in timeline)
            {
                if (!latestStatusUpdatedBy.ContainsKey(item.RepairTicketId)
                    && !string.IsNullOrWhiteSpace(item.ChangedByName))
                {
                    latestStatusUpdatedBy[item.RepairTicketId] = item.ChangedByName.Trim();
                }
            }
        }

        ViewData["LatestStatusUpdatedBy"] = latestStatusUpdatedBy;

        return View(tickets);
    }

    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.ITSupport)]
    public async Task<IActionResult> Report(string? status, string? keyword, bool print = false, int? ticketId = null)
    {
        RepairTicketReportViewModel model;
        if (ticketId.HasValue)
        {
            model = await BuildReportModelByIdAsync(ticketId.Value);

            if (print)
            {
                var printTimeline = await _context.RepairTicketStatusHistories
                    .AsNoTracking()
                    .Where(history => history.RepairTicketId == ticketId.Value)
                    .OrderBy(history => history.ChangedAt)
                    .ThenBy(history => history.Id)
                    .ToListAsync();

                ViewData["PrintTimeline"] = printTimeline;

                var itSupportUserIds = (await _userManager.GetUsersInRoleAsync(AppRoles.ITSupport))
                    .Select(user => user.Id)
                    .ToHashSet(StringComparer.Ordinal);
                var approveUserIds = (await _userManager.GetUsersInRoleAsync(AppRoles.Approve))
                    .Select(user => user.Id)
                    .ToHashSet(StringComparer.Ordinal);
                var itApproverUserIds = itSupportUserIds
                    .Intersect(approveUserIds)
                    .ToHashSet(StringComparer.Ordinal);

                if (itApproverUserIds.Count > 0)
                {
                    var itApproverEvent = printTimeline
                        .LastOrDefault(history =>
                            !string.IsNullOrWhiteSpace(history.ChangedByUserId)
                            && itApproverUserIds.Contains(history.ChangedByUserId)
                            && (history.Action == "Closed" || history.ToStatus == TicketStatus.Closed));

                    if (itApproverEvent is not null)
                    {
                        ViewData["PrintItApproverName"] = itApproverEvent.ChangedByName;
                        ViewData["PrintItApproverDate"] = itApproverEvent.ChangedAt;
                    }
                }
            }
        }
        else
        {
            model = await BuildReportModelAsync(status, keyword);
        }
        ViewData["PrintMode"] = print;

        return View(model);
    }

    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.ITSupport)]
    public IActionResult ExportReportPdf(string? status, string? keyword)
    {
        // Keep existing button routes but open the report in preview mode instead of browser print dialog.
        return RedirectToAction(nameof(Report), new { status, keyword, print = true });
    }

    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.ITSupport)]
    public IActionResult ExportTicketPdf(int id)
    {
        return RedirectToAction(nameof(Report), new { ticketId = id, print = true });
    }

    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.ITSupport)]
    public async Task<IActionResult> PerformanceReport(DateTime? fromDate, DateTime? toDate, string? itName)
    {
        var model = await BuildPerformanceReportModelAsync(fromDate, toDate, itName);
        return View(model);
    }

    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.ITSupport)]
    public async Task<IActionResult> ExportReportExcel()
    {
        var model = await BuildReportModelAsync(null, null);

        var csv = new StringBuilder();
        csv.AppendLine("Id,DocumentNo,RequesterName,Department,RepairType,Priority,Status,AssignedItName,ApproverName,CreatedAt,UpdatedAt,LastChangedAt,LastChangedByName,LastAction,LastRemark,IssueDescription");

        foreach (var item in model.Items)
        {
            csv.AppendLine(string.Join(",",
                CsvEscape(item.Id.ToString()),
                CsvEscape(item.DocumentNo),
                CsvEscape(item.RequesterName),
                CsvEscape(item.Department),
                CsvEscape(item.RepairType.ToString()),
                CsvEscape(item.Priority.ToString()),
                CsvEscape(item.Status.ToString()),
                CsvEscape(item.AssignedItName),
                CsvEscape(item.ApproverName),
                CsvEscape(item.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")),
                CsvEscape(item.UpdatedAt?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty),
                CsvEscape(item.LastChangedAt?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty),
                CsvEscape(item.LastChangedByName),
                CsvEscape(item.LastAction),
                CsvEscape(item.LastRemark),
                CsvEscape(item.IssueDescription)));
        }

        var content = "\uFEFF" + csv;
        var bytes = Encoding.UTF8.GetBytes(content);
        var fileName = $"RepairTickets_Report_All_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

        return File(bytes, "text/csv; charset=utf-8", fileName);
    }

    private static string CsvEscape(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "\"\"";
        }

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    private async Task<RepairTicketPerformanceReportViewModel> BuildPerformanceReportModelAsync(DateTime? fromDate, DateTime? toDate, string? itName)
    {
        var query = _context.RepairTickets.AsNoTracking()
            .Where(ticket => ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.InProgress)
            .AsQueryable();

        var trimmedItName = itName?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(trimmedItName))
        {
            query = query.Where(ticket =>
                EF.Functions.Like(ticket.AssignedItName, $"%{trimmedItName}%")
                || EF.Functions.Like(ticket.AssignedItUserId, $"%{trimmedItName}%"));
        }

        var tickets = await query
            .OrderByDescending(ticket => ticket.CreatedAt)
            .ToListAsync();

        var assigneeUserIds = tickets
            .Where(ticket => string.IsNullOrWhiteSpace(ticket.AssignedItName) && !string.IsNullOrWhiteSpace(ticket.AssignedItUserId))
            .Select(ticket => ticket.AssignedItUserId)
            .Distinct()
            .ToList();

        var assigneeNameByUserId = assigneeUserIds.Count > 0
            ? await _context.Users.AsNoTracking()
                .Where(user => assigneeUserIds.Contains(user.Id))
                .Select(user => new { user.Id, user.FullName, user.UserName, user.Email })
                .ToDictionaryAsync(
                    user => user.Id,
                    user => !string.IsNullOrWhiteSpace(user.FullName)
                        ? user.FullName
                        : (user.UserName ?? user.Email ?? user.Id))
            : new Dictionary<string, string>();

        var ticketIds = tickets.Select(ticket => ticket.Id).ToList();
        var histories = ticketIds.Count == 0
            ? new List<RepairTicketStatusHistory>()
            : await _context.RepairTicketStatusHistories.AsNoTracking()
                .Where(history => ticketIds.Contains(history.RepairTicketId))
                .OrderBy(history => history.ChangedAt)
                .ThenBy(history => history.Id)
                .ToListAsync();

        var historiesByTicketId = histories
            .GroupBy(history => history.RepairTicketId)
            .ToDictionary(group => group.Key, group => group.ToList());

        var items = new List<RepairTicketPerformanceCaseViewModel>();
        var nowUtc = DateTime.UtcNow;
        foreach (var ticket in tickets)
        {
            if (!historiesByTicketId.TryGetValue(ticket.Id, out var ticketHistory))
            {
                continue;
            }

            var assignedEvent = ticketHistory
                .FirstOrDefault(history => history.ToStatus == TicketStatus.InProgress);
            var closedEvent = ticketHistory
                .LastOrDefault(history => history.ToStatus == TicketStatus.Closed || history.Action == "Closed");

            if (assignedEvent is null)
            {
                continue;
            }

            var endAtUtc = closedEvent?.ChangedAt ?? nowUtc;
            if (endAtUtc < assignedEvent.ChangedAt)
            {
                continue;
            }

            var assignedItName = !string.IsNullOrWhiteSpace(ticket.AssignedItName)
                ? ticket.AssignedItName
                : (!string.IsNullOrWhiteSpace(ticket.AssignedItUserId)
                    && assigneeNameByUserId.TryGetValue(ticket.AssignedItUserId, out var resolvedName)
                        ? resolvedName
                        : "-");

            var duration = endAtUtc - assignedEvent.ChangedAt;
            var referenceLocalDate = endAtUtc.ToLocalTime().Date;
            if (fromDate.HasValue && referenceLocalDate < fromDate.Value.Date)
            {
                continue;
            }

            if (toDate.HasValue && referenceLocalDate > toDate.Value.Date)
            {
                continue;
            }

            items.Add(new RepairTicketPerformanceCaseViewModel
            {
                TicketId = ticket.Id,
                DocumentNo = ticket.DocumentNo ?? string.Empty,
                RequesterName = ticket.RequesterName,
                Department = ticket.Department,
                AssignedItName = assignedItName,
                AssignedAt = assignedEvent.ChangedAt,
                ClosedAt = closedEvent?.ChangedAt,
                CurrentStatus = ticket.Status,
                Duration = duration
            });
        }

        var summary = items
            .GroupBy(item => string.IsNullOrWhiteSpace(item.AssignedItName) ? "-" : item.AssignedItName)
            .Select(group => new RepairTicketPerformanceStaffSummaryViewModel
            {
                ItStaffName = group.Key,
                ClosedCases = group.Count(),
                AverageHours = group.Average(item => item.Duration.TotalHours),
                MinimumHours = group.Min(item => item.Duration.TotalHours),
                MaximumHours = group.Max(item => item.Duration.TotalHours)
            })
            .OrderBy(summaryItem => summaryItem.AverageHours)
            .ThenBy(summaryItem => summaryItem.ItStaffName)
            .ToList();

        return new RepairTicketPerformanceReportViewModel
        {
            FromDate = fromDate,
            ToDate = toDate,
            ItName = trimmedItName,
            Cases = items.OrderByDescending(item => item.ClosedAt).ToList(),
            StaffSummaries = summary
        };
    }

    private async Task<RepairTicketReportViewModel> BuildReportModelByIdAsync(int id)
    {
        var ticket = await _context.RepairTickets.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket is null)
        {
            return new RepairTicketReportViewModel { Items = [], StatusCounts = [] };
        }

        var latestHistory = await _context.RepairTicketStatusHistories.AsNoTracking()
            .Where(h => h.RepairTicketId == id)
            .OrderByDescending(h => h.ChangedAt).ThenByDescending(h => h.Id)
            .FirstOrDefaultAsync();

        var assignedItName = ticket.AssignedItName;
        if (string.IsNullOrWhiteSpace(assignedItName) && !string.IsNullOrWhiteSpace(ticket.AssignedItUserId))
        {
            var assignee = await _context.Users.AsNoTracking()
                .Where(user => user.Id == ticket.AssignedItUserId)
                .Select(user => new { user.FullName, user.UserName, user.Email })
                .FirstOrDefaultAsync();

            if (assignee is not null)
            {
                assignedItName = !string.IsNullOrWhiteSpace(assignee.FullName)
                    ? assignee.FullName
                    : (assignee.UserName ?? assignee.Email ?? ticket.AssignedItUserId);
            }
            else
            {
                assignedItName = ticket.AssignedItUserId;
            }
        }

        var item = new RepairTicketReportItemViewModel
        {
            Id = ticket.Id,
            DocumentNo = ticket.DocumentNo ?? string.Empty,
            RequesterName = ticket.RequesterName,
            Department = ticket.Department,
            DeviceName = ticket.DeviceName,
            IssueDescription = ticket.IssueDescription,
            RepairType = ticket.RepairType,
            Priority = ticket.Priority,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            ApproverName = ticket.ApproverName,
            AssignedItName = assignedItName ?? string.Empty,
            LastChangedAt = latestHistory?.ChangedAt,
            LastChangedByName = latestHistory?.ChangedByName ?? string.Empty,
            LastAction = latestHistory?.Action ?? string.Empty,
            LastRemark = latestHistory?.Remark ?? string.Empty
        };

        return new RepairTicketReportViewModel
        {
            Items = [item],
            StatusCounts = Enum.GetValues<TicketStatus>().ToDictionary(s => s, s => 0)
        };
    }

    private async Task<RepairTicketReportViewModel> BuildReportModelAsync(string? status, string? keyword)
    {
        var query = _context.RepairTickets.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TicketStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(ticket => ticket.Status == parsedStatus);
        }

        var trimmedKeyword = keyword?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(trimmedKeyword))
        {
            query = query.Where(ticket =>
                EF.Functions.Like(ticket.RequesterName, $"%{trimmedKeyword}%")
                || EF.Functions.Like(ticket.Department, $"%{trimmedKeyword}%")
                || EF.Functions.Like(ticket.DeviceName, $"%{trimmedKeyword}%")
                || EF.Functions.Like(ticket.IssueDescription, $"%{trimmedKeyword}%")
                || EF.Functions.Like(ticket.ApproverName, $"%{trimmedKeyword}%")
                || EF.Functions.Like(ticket.AssignedItName, $"%{trimmedKeyword}%")
                || EF.Functions.Like(ticket.DocumentNo ?? string.Empty, $"%{trimmedKeyword}%"));
        }

        var tickets = await query
            .OrderByDescending(ticket => ticket.CreatedAt)
            .ToListAsync();

        var latestHistoryByTicketId = new Dictionary<int, RepairTicketStatusHistory>();
        if (tickets.Count > 0)
        {
            var ticketIds = tickets.Select(ticket => ticket.Id).ToList();
            var histories = await _context.RepairTicketStatusHistories
                .AsNoTracking()
                .Where(history => ticketIds.Contains(history.RepairTicketId))
                .OrderByDescending(history => history.ChangedAt)
                .ThenByDescending(history => history.Id)
                .ToListAsync();

            foreach (var history in histories)
            {
                if (!latestHistoryByTicketId.ContainsKey(history.RepairTicketId))
                {
                    latestHistoryByTicketId[history.RepairTicketId] = history;
                }
            }
        }

        var assigneeUserIds = tickets
            .Where(ticket => string.IsNullOrWhiteSpace(ticket.AssignedItName) && !string.IsNullOrWhiteSpace(ticket.AssignedItUserId))
            .Select(ticket => ticket.AssignedItUserId)
            .Distinct()
            .ToList();

        var assigneeNameByUserId = assigneeUserIds.Count > 0
            ? await _context.Users.AsNoTracking()
                .Where(user => assigneeUserIds.Contains(user.Id))
                .Select(user => new { user.Id, user.FullName, user.UserName, user.Email })
                .ToDictionaryAsync(
                    user => user.Id,
                    user => !string.IsNullOrWhiteSpace(user.FullName)
                        ? user.FullName
                        : (user.UserName ?? user.Email ?? user.Id))
            : new Dictionary<string, string>();

        return new RepairTicketReportViewModel
        {
            CurrentStatus = status,
            Keyword = trimmedKeyword,
            StatusCounts = Enum.GetValues<TicketStatus>()
                .ToDictionary(
                    ticketStatus => ticketStatus,
                    ticketStatus => tickets.Count(ticket => ticket.Status == ticketStatus)),
            Items = tickets.Select(ticket =>
            {
                latestHistoryByTicketId.TryGetValue(ticket.Id, out var latestHistory);
                var assignedItName = ticket.AssignedItName;
                if (string.IsNullOrWhiteSpace(assignedItName) && !string.IsNullOrWhiteSpace(ticket.AssignedItUserId))
                {
                    assignedItName = assigneeNameByUserId.TryGetValue(ticket.AssignedItUserId, out var resolvedName)
                        ? resolvedName
                        : ticket.AssignedItUserId;
                }

                return new RepairTicketReportItemViewModel
                {
                    Id = ticket.Id,
                    DocumentNo = ticket.DocumentNo ?? string.Empty,
                    RequesterName = ticket.RequesterName,
                    Department = ticket.Department,
                    DeviceName = ticket.DeviceName,
                    IssueDescription = ticket.IssueDescription,
                    RepairType = ticket.RepairType,
                    Priority = ticket.Priority,
                    Status = ticket.Status,
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt,
                    ApproverName = ticket.ApproverName,
                    AssignedItName = assignedItName ?? string.Empty,
                    LastChangedAt = latestHistory?.ChangedAt,
                    LastChangedByName = latestHistory?.ChangedByName ?? string.Empty,
                    LastAction = latestHistory?.Action ?? string.Empty,
                    LastRemark = latestHistory?.Remark ?? string.Empty
                };
            }).ToList()
        };
    }

    public async Task<IActionResult> Details(int? id)
    {
        return await RenderDetailsViewAsync(id, "Details");
    }

    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.ITSupport + "," + AppRoles.Approve)]
    public async Task<IActionResult> ReportDetails(int? id)
    {
        return await RenderDetailsViewAsync(id, "ReportDetails");
    }

    private async Task<IActionResult> RenderDetailsViewAsync(int? id, string viewName)
    {
        if (id is null)
        {
            return NotFound();
        }

        var ticket = await _context.RepairTickets
            .FirstOrDefaultAsync(ticket => ticket.Id == id);

        if (ticket is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(ticket.AssignedItName) && !string.IsNullOrWhiteSpace(ticket.AssignedItUserId))
        {
            var assignee = await _context.Users.AsNoTracking()
                .Where(user => user.Id == ticket.AssignedItUserId)
                .Select(user => new { user.FullName, user.UserName, user.Email })
                .FirstOrDefaultAsync();

            ticket.AssignedItName = assignee is null
                ? ticket.AssignedItUserId
                : (!string.IsNullOrWhiteSpace(assignee.FullName)
                    ? assignee.FullName
                    : (assignee.UserName ?? assignee.Email ?? ticket.AssignedItUserId));
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (ticket.Status == TicketStatus.Rejected && currentUser is not null && IsOwnerTicket(ticket, currentUser))
        {
            var latestRejectedAt = await _context.RepairTicketStatusHistories
                .AsNoTracking()
                .Where(history => history.RepairTicketId == ticket.Id && history.ToStatus == TicketStatus.Rejected)
                .OrderByDescending(history => history.ChangedAt)
                .ThenByDescending(history => history.Id)
                .Select(history => (DateTime?)history.ChangedAt)
                .FirstOrDefaultAsync();

            if (latestRejectedAt.HasValue)
            {
                var hasReadAfterLatestReject = await _context.RepairTicketStatusHistories
                    .AsNoTracking()
                    .AnyAsync(history =>
                        history.RepairTicketId == ticket.Id
                        && history.Action == "RejectedRead"
                        && history.ChangedByUserId == currentUser.Id
                        && history.ChangedAt >= latestRejectedAt.Value);

                if (!hasReadAfterLatestReject)
                {
                    ticket.UpdatedAt = DateTime.UtcNow;
                    ticket.UpdatedByName = GetActorName(currentUser);
                    AddStatusHistory(ticket, ticket.Status, ticket.Status, currentUser, "RejectedRead");
                    await _context.SaveChangesAsync();
                }
            }
        }

        var canSeeInProgress = User.IsInRole(AppRoles.Admin) || User.IsInRole(AppRoles.ITSupport);
        var canEdit = User.IsInRole(AppRoles.ITSupport)
            || User.IsInRole(AppRoles.Admin)
            || User.IsInRole(AppRoles.Approve)
            || (ticket.Status == TicketStatus.Rejected && IsOwnerTicket(ticket, currentUser));
        if (!canSeeInProgress && ticket.Status == TicketStatus.InProgress)
        {
            canEdit = false;
        }

        ViewData["CanEditTicket"] = canEdit;

        var timeline = await _context.RepairTicketStatusHistories
            .AsNoTracking()
            .Where(history => history.RepairTicketId == ticket.Id)
            .OrderByDescending(history => history.ChangedAt)
            .ToListAsync();
        ViewData["StatusTimeline"] = timeline;

        return View(viewName, ticket);
    }

    public async Task<IActionResult> Create()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        ViewData["CurrentUserId"] = currentUser?.Id;
        var ticket = new RepairTicket
        {
            RequesterName = !string.IsNullOrWhiteSpace(currentUser?.FullName)
                ? currentUser.FullName
                : (currentUser?.UserName ?? User.Identity?.Name ?? string.Empty),
            Department = currentUser?.Department ?? string.Empty,
            Status = TicketStatus.Open
        };

        await PopulateApproverSelectionsAsync(ticket.ApproverDepartment, ticket.ApproverUserId);
        await PopulateDriveAccessDepartmentSelectionsAsync(ticket.DriveAccessDepartment);
        return View(ticket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("RequesterName,Department,DeviceName,IssueDescription,RepairType,DriveAccessDepartment,Priority,Status,ApproverDepartment,ApproverUserId")] RepairTicket ticket)
    {
        ticket.Priority = TicketPriority.Medium;

        var canSetStatus = User.IsInRole(AppRoles.ITSupport) || User.IsInRole(AppRoles.Admin);
        var isApproveCreator = User.IsInRole(AppRoles.Approve);
        if (isApproveCreator && !canSetStatus)
        {
            // Approver-created tickets move through Open -> Approved immediately.
            ticket.Status = TicketStatus.Approved;
        }
        else if (!canSetStatus)
        {
            ticket.Status = TicketStatus.Open;
        }

        var currentUser = await _userManager.GetUserAsync(User);
        ticket.RequesterName = !string.IsNullOrWhiteSpace(currentUser?.FullName)
            ? currentUser.FullName
            : (currentUser?.UserName ?? User.Identity?.Name ?? string.Empty);

        ticket.Department = ticket.Department?.Trim() ?? string.Empty;
        ticket.ApproverDepartment = ticket.ApproverDepartment?.Trim() ?? string.Empty;
        ticket.DriveAccessDepartment = ticket.DriveAccessDepartment?.Trim() ?? string.Empty;
        ticket.IssueDescription = ticket.IssueDescription?.Trim() ?? string.Empty;

        var targetApproverDepartment = ticket.RepairType == RepairType.DriveAccessPermission
            ? ticket.DriveAccessDepartment
            : ticket.Department;

        var approverUsers = await GetApproverUsersAsync();
        var selectedApprover = approverUsers.FirstOrDefault(user => user.Id == ticket.ApproverUserId);

        if (ticket.RepairType != RepairType.DriveAccessPermission)
        {
            ticket.DriveAccessDepartment = string.Empty;
        }

        if (ticket.RepairType == RepairType.DriveAccessPermission)
        {
            var driveDepartment = ticket.DriveAccessDepartment.Trim();
            var driveRemark = $"ฝ่ายที่ต้องการขอสิทธิ์ Drive: {driveDepartment}";
            if (string.IsNullOrWhiteSpace(ticket.IssueDescription))
            {
                ticket.IssueDescription = driveRemark;
            }
            else if (!ticket.IssueDescription.Contains(driveRemark, StringComparison.OrdinalIgnoreCase))
            {
                ticket.IssueDescription = $"{driveRemark}\n{ticket.IssueDescription}";
            }

            // The model binder may have added a required error before this auto-fill ran.
            ModelState.Remove(nameof(RepairTicket.IssueDescription));
        }

        if (selectedApprover is null && !string.IsNullOrWhiteSpace(targetApproverDepartment))
        {
            selectedApprover = approverUsers.FirstOrDefault(user =>
                string.Equals(user.Department?.Trim(), targetApproverDepartment, StringComparison.OrdinalIgnoreCase));

            if (selectedApprover is not null)
            {
                ticket.ApproverUserId = selectedApprover.Id;
            }
        }

        if (selectedApprover is null)
        {
            ModelState.AddModelError(nameof(RepairTicket.ApproverUserId), "Please select an approver.");
        }
        else
        {
            ticket.ApproverName = !string.IsNullOrWhiteSpace(selectedApprover.FullName)
                ? selectedApprover.FullName
                : (selectedApprover.UserName ?? selectedApprover.Email ?? "Unknown");

            var approverDepartment = selectedApprover.Department?.Trim() ?? string.Empty;
            if (!string.Equals(ticket.ApproverDepartment?.Trim(), approverDepartment, StringComparison.OrdinalIgnoreCase))
            {
                ticket.ApproverDepartment = approverDepartment;
            }
        }

        if (User.IsInRole(AppRoles.Approve)
            && !User.IsInRole(AppRoles.ITSupport)
            && !string.IsNullOrWhiteSpace(currentUser?.Id)
            && string.Equals(ticket.ApproverUserId, currentUser.Id, StringComparison.Ordinal))
        {
            ModelState.AddModelError(nameof(RepairTicket.ApproverUserId), "ผู้รับ step ถัดไปต้องไม่เป็นผู้ใช้คนเดียวกับผู้ที่กำลังอนุมัติ กรุณาเลือกผู้รับคนอื่น");
        }

        if (ticket.Status == TicketStatus.Rejected)
        {
            ModelState.AddModelError(nameof(RepairTicket.Status), "เจ้าของรายการไม่สามารถ Rejected รายการของตนเองได้");
        }

        if (ticket.IssueDescription.Length > 500)
        {
            ModelState.AddModelError(nameof(RepairTicket.IssueDescription), "รายละเอียดปัญหาต้องไม่เกิน 500 ตัวอักษร");
        }

        if (!ModelState.IsValid)
        {
            ViewData["CurrentUserId"] = currentUser?.Id;
            await PopulateApproverSelectionsAsync(ticket.ApproverDepartment, ticket.ApproverUserId);
            await PopulateDriveAccessDepartmentSelectionsAsync(ticket.DriveAccessDepartment);
            return View(ticket);
        }

        var actorName = GetActorName(currentUser);
        ticket.CreatedAt = DateTime.UtcNow;
        ticket.CreatedByName = actorName;
        ticket.RequesterUserId = currentUser?.Id;
        ticket.DocumentNo = await GenerateDocumentNo(ticket.CreatedAt);
        _context.Add(ticket);

        if (isApproveCreator && !canSetStatus && ticket.Status == TicketStatus.Approved)
        {
            AddStatusHistory(ticket, null, TicketStatus.Open, currentUser, "Created");
            AddStatusHistory(ticket, TicketStatus.Open, TicketStatus.Approved, currentUser, "StatusChanged");
        }
        else
        {
            AddStatusHistory(ticket, null, ticket.Status, currentUser, "Created");
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var ticket = await _context.RepairTickets.FindAsync(id);
        if (ticket is null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        ViewData["CurrentUserId"] = currentUser?.Id;
        ViewData["CurrentUserDisplayName"] = GetActorName(currentUser);
        var canSeeInProgress = User.IsInRole(AppRoles.Admin) || User.IsInRole(AppRoles.ITSupport);
        var isPrivilegedUser = User.IsInRole(AppRoles.ITSupport)
            || User.IsInRole(AppRoles.Admin)
            || User.IsInRole(AppRoles.Approve);
        var isOwner = IsOwnerTicket(ticket, currentUser);

        if (!isPrivilegedUser)
        {
            if (!isOwner || (ticket.Status != TicketStatus.Rejected && ticket.Status != TicketStatus.Open))
            {
                return Forbid();
            }

            ViewData["RequesterEditMode"] = true;
        }

        if (!canSeeInProgress && ticket.Status == TicketStatus.InProgress)
        {
            return Forbid();
        }

        var timelineEntries = await _context.RepairTicketStatusHistories
            .AsNoTracking()
            .Where(history => history.RepairTicketId == ticket.Id)
            .OrderBy(history => history.ChangedAt)
            .ThenBy(history => history.Id)
            .Select(history => new { history.ToStatus, history.ChangedByUserId })
            .ToListAsync();

        var latestStatusEditorUserId = timelineEntries.LastOrDefault()?.ChangedByUserId;
        var canEditStatusByUser = string.IsNullOrWhiteSpace(latestStatusEditorUserId)
            || string.Equals(latestStatusEditorUserId, currentUser?.Id, StringComparison.Ordinal);
        var canMarkCompleteByAssignee = currentUser is not null
            && !string.IsNullOrWhiteSpace(ticket.AssignedItUserId)
            && string.Equals(ticket.AssignedItUserId, currentUser.Id, StringComparison.Ordinal)
            && ticket.Status == TicketStatus.InProgress;

        ViewData["CanEditStatusByUser"] = canEditStatusByUser;
        ViewData["CanMarkCompleteByAssignee"] = canMarkCompleteByAssignee;

        var timelineStatuses = CollapseConsecutiveStatuses(timelineEntries.Select(entry => entry.ToStatus));

        var statusFlow = timelineStatuses.Count > 0
            ? new List<TicketStatus>(timelineStatuses)
            : new List<TicketStatus> { ticket.Status };

        if (statusFlow[0] != TicketStatus.Open)
        {
            statusFlow.Insert(0, TicketStatus.Open);
        }

        if (statusFlow[^1] != ticket.Status)
        {
            statusFlow.Add(ticket.Status);
        }

        statusFlow = CollapseConsecutiveStatuses(statusFlow);

        ViewData["ShowInitialOpenStatus"] = statusFlow.Count > 1 && statusFlow[0] == TicketStatus.Open;
        var editableStatusFlow = GetEditableStatusFlow(statusFlow);
        ViewData["HasPriorStatusSteps"] = editableStatusFlow.Count > 1;
        ViewData["FlowPrimaryStatus"] = editableStatusFlow[0];
        ViewData["ApprovalStatuses"] = editableStatusFlow.Skip(1).ToList();

        await PopulateItSupportSelectionsAsync(ticket.AssignedItUserId);
        await PopulateApproverSelectionsAsync(ticket.ApproverDepartment, ticket.ApproverUserId);
        return View(ticket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,RequesterName,Department,DeviceName,IssueDescription,RepairType,Priority,Status,CreatedAt,ApproverDepartment,ApproverUserId,ApproverName,AssignedItUserId")] RepairTicket ticket, string? rejectRemark, List<TicketStatus>? approvalStatuses)
    {
        if (id != ticket.Id)
        {
            return NotFound();
        }

        var existingTicket = await _context.RepairTickets.FirstOrDefaultAsync(existing => existing.Id == id);
        if (existingTicket is null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var canSeeInProgress = User.IsInRole(AppRoles.Admin) || User.IsInRole(AppRoles.ITSupport);
        var isPrivilegedUser = User.IsInRole(AppRoles.ITSupport)
            || User.IsInRole(AppRoles.Admin)
            || User.IsInRole(AppRoles.Approve);
        var canEditStatus = User.IsInRole(AppRoles.Approve);
        var originalStatus = existingTicket.Status;
        var canMarkCompleteByAssignee = currentUser is not null
            && !string.IsNullOrWhiteSpace(existingTicket.AssignedItUserId)
            && string.Equals(existingTicket.AssignedItUserId, currentUser.Id, StringComparison.Ordinal)
            && originalStatus == TicketStatus.InProgress;
        var isOwner = IsOwnerTicket(existingTicket, currentUser);
        var persistedTimelineStatuses = await _context.RepairTicketStatusHistories
            .AsNoTracking()
            .Where(history => history.RepairTicketId == existingTicket.Id)
            .OrderBy(history => history.ChangedAt)
            .ThenBy(history => history.Id)
            .Select(history => history.ToStatus)
            .ToListAsync();
        persistedTimelineStatuses = CollapseConsecutiveStatuses(persistedTimelineStatuses);
        var latestStatusEditorUserId = await _context.RepairTicketStatusHistories
            .AsNoTracking()
            .Where(history => history.RepairTicketId == existingTicket.Id)
            .OrderByDescending(history => history.ChangedAt)
            .ThenByDescending(history => history.Id)
            .Select(history => history.ChangedByUserId)
            .FirstOrDefaultAsync();
        var canEditStatusByUser = string.IsNullOrWhiteSpace(latestStatusEditorUserId)
            || string.Equals(latestStatusEditorUserId, currentUser?.Id, StringComparison.Ordinal);
        var persistedStatusFlow = persistedTimelineStatuses.Count > 0
            ? new List<TicketStatus>(persistedTimelineStatuses)
            : new List<TicketStatus> { existingTicket.Status };

        if (persistedStatusFlow[0] != TicketStatus.Open)
        {
            persistedStatusFlow.Insert(0, TicketStatus.Open);
        }

        if (persistedStatusFlow[^1] != existingTicket.Status)
        {
            persistedStatusFlow.Add(existingTicket.Status);
        }

        var persistedEditableStatusFlow = GetEditableStatusFlow(persistedStatusFlow);
        var persistedStatusStepCount = persistedEditableStatusFlow.Count - 1;
        var submittedApprovalStatuses = (approvalStatuses ?? new List<TicketStatus>())
            .Where(status => status != TicketStatus.Closed && status != TicketStatus.Open)
            .ToList();

        if (ticket.Status == TicketStatus.Open && originalStatus != TicketStatus.Open)
        {
            return Forbid();
        }

        var attemptedStatusChange = ticket.Status != originalStatus
            || submittedApprovalStatuses.Count > 0
            || !string.IsNullOrWhiteSpace(rejectRemark);

        var assigneeCompleteRequest = canMarkCompleteByAssignee
            && ticket.Status == TicketStatus.Complete
            && submittedApprovalStatuses.Count == 0
            && string.IsNullOrWhiteSpace(rejectRemark);

        if (!canEditStatus && attemptedStatusChange && !assigneeCompleteRequest)
        {
            return Forbid();
        }

        if (!canEditStatus && !assigneeCompleteRequest)
        {
            ticket.Status = originalStatus;
            submittedApprovalStatuses.Clear();
        }

        var statusFlow = CollapseConsecutiveStatuses(new[] { ticket.Status }.Concat(submittedApprovalStatuses));

        if (!canEditStatusByUser && !assigneeCompleteRequest)
        {
            if (statusFlow.Count < persistedEditableStatusFlow.Count)
            {
                return Forbid();
            }

            var hasSamePrefix = statusFlow
                .Take(persistedEditableStatusFlow.Count)
                .SequenceEqual(persistedEditableStatusFlow);

            if (!hasSamePrefix)
            {
                return Forbid();
            }
        }

        var effectiveStatus = statusFlow.Last();

        if (!isPrivilegedUser)
        {
            if (!isOwner || (existingTicket.Status != TicketStatus.Rejected && existingTicket.Status != TicketStatus.Open))
            {
                return Forbid();
            }

            existingTicket.Department = ticket.Department;
            existingTicket.DeviceName = ticket.DeviceName;
            existingTicket.IssueDescription = ticket.IssueDescription;
            existingTicket.RepairType = ticket.RepairType;
            existingTicket.Status = originalStatus;
            existingTicket.UpdatedAt = DateTime.UtcNow;
            existingTicket.UpdatedByName = GetActorName(currentUser);

            if (!TryValidateModel(existingTicket))
            {
                ViewData["RequesterEditMode"] = true;
                await PopulateItSupportSelectionsAsync(existingTicket.AssignedItUserId);
                await PopulateApproverSelectionsAsync(existingTicket.ApproverDepartment, existingTicket.ApproverUserId);
                return View(existingTicket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        if (!canSeeInProgress && (existingTicket.Status == TicketStatus.InProgress || statusFlow.Any(status => status == TicketStatus.InProgress)))
        {
            return Forbid();
        }

        existingTicket.RequesterName = ticket.RequesterName;
        existingTicket.Department = ticket.Department;
        existingTicket.DeviceName = ticket.DeviceName;
        existingTicket.IssueDescription = ticket.IssueDescription;
        existingTicket.RepairType = ticket.RepairType;
        existingTicket.Priority = ticket.Priority;
        existingTicket.Status = effectiveStatus;
        existingTicket.CreatedAt = ticket.CreatedAt;
        existingTicket.ApproverDepartment = ticket.ApproverDepartment;
        existingTicket.ApproverUserId = ticket.ApproverUserId;
        existingTicket.ApproverName = ticket.ApproverName;
        existingTicket.AssignedItUserId = ticket.AssignedItUserId?.Trim() ?? string.Empty;

        if (User.IsInRole(AppRoles.Approve)
            && !User.IsInRole(AppRoles.ITSupport)
            && !string.IsNullOrWhiteSpace(currentUser?.Id)
            && string.Equals(existingTicket.ApproverUserId, currentUser.Id, StringComparison.Ordinal))
        {
            ModelState.AddModelError(nameof(RepairTicket.ApproverUserId), "ผู้รับ step ถัดไปต้องไม่เป็นผู้ใช้คนเดียวกับผู้ที่กำลังอนุมัติ กรุณาเลือกผู้รับคนอื่น");
        }

        if (effectiveStatus == TicketStatus.InProgress)
        {
            if (string.IsNullOrWhiteSpace(existingTicket.AssignedItUserId))
            {
                ModelState.AddModelError(nameof(RepairTicket.AssignedItUserId), "Please select IT assignee when status is InProgress.");
            }
            else
            {
                var assignedIt = await _userManager.FindByIdAsync(existingTicket.AssignedItUserId);
                if (assignedIt is not null)
                {
                    existingTicket.AssignedItName = !string.IsNullOrWhiteSpace(assignedIt.FullName)
                        ? assignedIt.FullName
                        : (assignedIt.UserName ?? assignedIt.Email ?? "Unknown");
                }
                else if (string.IsNullOrWhiteSpace(existingTicket.AssignedItName))
                {
                    existingTicket.AssignedItName = existingTicket.AssignedItUserId;
                }
            }
        }
        else
        {
            existingTicket.AssignedItUserId = string.Empty;
            existingTicket.AssignedItName = string.Empty;
        }

        var trimmedRejectRemark = rejectRemark?.Trim() ?? string.Empty;

        if (effectiveStatus == TicketStatus.Rejected && isOwner)
        {
            ModelState.AddModelError(nameof(RepairTicket.Status), "เจ้าของรายการไม่สามารถ Rejected รายการของตนเองได้");
        }

        if (effectiveStatus == TicketStatus.Rejected)
        {
            if (string.IsNullOrWhiteSpace(trimmedRejectRemark))
            {
                ModelState.AddModelError("rejectRemark", "Please provide a rejection remark when status is Rejected.");
            }
            else
            {
                var returnMessage = trimmedRejectRemark;
                var updatedIssueDescription = string.IsNullOrWhiteSpace(existingTicket.IssueDescription)
                    ? returnMessage
                    : $"{existingTicket.IssueDescription}\n\n{returnMessage}";

                if (updatedIssueDescription.Length > 500)
                {
                    ModelState.AddModelError(nameof(RepairTicket.IssueDescription), "Issue description is too long after adding rejection remark. Please shorten the remark.");
                }
                else
                {
                    existingTicket.IssueDescription = updatedIssueDescription;
                }
            }
        }

        if (!ModelState.IsValid)
        {
            ViewData["RejectRemark"] = trimmedRejectRemark;
            ViewData["HasPriorStatusSteps"] = persistedStatusStepCount > 0;
            ViewData["ShowInitialOpenStatus"] = persistedStatusFlow.Count > 1 && persistedStatusFlow[0] == TicketStatus.Open;
            ViewData["CanEditStatusByUser"] = canEditStatusByUser;
            ViewData["CanMarkCompleteByAssignee"] = canMarkCompleteByAssignee;
            ViewData["ApprovalStatuses"] = statusFlow.Skip(1).ToList();
            ViewData["CurrentUserId"] = currentUser?.Id;
            ViewData["CurrentUserDisplayName"] = GetActorName(currentUser);
            await PopulateItSupportSelectionsAsync(existingTicket.AssignedItUserId);
            await PopulateApproverSelectionsAsync(existingTicket.ApproverDepartment, existingTicket.ApproverUserId);
            return View(existingTicket);
        }

        try
        {
            existingTicket.UpdatedAt = DateTime.UtcNow;
            existingTicket.UpdatedByName = GetActorName(currentUser);

            if (statusFlow.Count > 1)
            {
                var fromStatus = originalStatus;
                var hasFlowChange = false;

                foreach (var stepStatus in statusFlow)
                {
                    if (stepStatus == fromStatus)
                    {
                        continue;
                    }

                    hasFlowChange = true;
                    var stepRemark = stepStatus == TicketStatus.Rejected
                        ? trimmedRejectRemark
                        : null;
                    AddStatusHistory(existingTicket, fromStatus, stepStatus, currentUser, "StatusChanged", stepRemark);
                    fromStatus = stepStatus;
                }

                if (!hasFlowChange)
                {
                    AddStatusHistory(existingTicket, existingTicket.Status, existingTicket.Status, currentUser, "Saved", null);
                }
            }
            else if (originalStatus != existingTicket.Status)
            {
                var statusRemark = existingTicket.Status == TicketStatus.Rejected
                    ? trimmedRejectRemark
                    : null;
                AddStatusHistory(existingTicket, originalStatus, existingTicket.Status, currentUser, "StatusChanged", statusRemark);
            }
            else
            {
                // Record every save even when status is unchanged (e.g. multiple approval steps)
                AddStatusHistory(existingTicket, existingTicket.Status, existingTicket.Status, currentUser, "Saved", null);
            }

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await RepairTicketExists(existingTicket.Id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var ticket = await _context.RepairTickets
            .AsNoTracking()
            .FirstOrDefaultAsync(ticket => ticket.Id == id);

        if (ticket is null)
        {
            return NotFound();
        }

        return View(ticket);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ticket = await _context.RepairTickets.FindAsync(id);
        if (ticket is not null)
        {
            _context.RepairTickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private Task<bool> RepairTicketExists(int id)
    {
        return _context.RepairTickets.AnyAsync(ticket => ticket.Id == id);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Approve)]
    public async Task<IActionResult> CloseTicket(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var ticket = await _context.RepairTickets.FindAsync(id);

        if (ticket is null)
        {
            return NotFound();
        }

        if (ticket.Status != TicketStatus.Complete)
        {
            return Forbid();
        }

        var previousStatus = ticket.Status;
        ticket.Status = TicketStatus.Closed;
        ticket.UpdatedAt = DateTime.UtcNow;
        ticket.UpdatedByName = GetActorName(currentUser);
        AddStatusHistory(ticket, previousStatus, ticket.Status, currentUser, "Closed");
        _context.Update(ticket);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    

    private async Task<List<ApplicationUser>> GetApproverUsersAsync()
    {
        var approvers = await _userManager.GetUsersInRoleAsync(AppRoles.Approve);
        return approvers
            .OrderBy(user => user.Department)
            .ThenBy(user => user.FullName)
            .ThenBy(user => user.UserName)
            .ToList();
    }

    private async Task<List<ApplicationUser>> GetItSupportUsersAsync()
    {
        var itSupportUsers = await _userManager.GetUsersInRoleAsync(AppRoles.ITSupport);

        return itSupportUsers
            .OrderBy(user => user.Department)
            .ThenBy(user => user.FullName)
            .ThenBy(user => user.UserName)
            .ToList();
    }

    private async Task PopulateApproverSelectionsAsync(string? selectedDepartment, string? selectedApproverId)
    {
        var approverUsers = await GetApproverUsersAsync();
        var departments = (await _context.Users
                .AsNoTracking()
                .ToListAsync())
            .Select(user => user.Department?.Trim() ?? string.Empty)
            .Where(department => !string.IsNullOrWhiteSpace(department))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(department => department)
            .ToList();

        ViewData["ApproverDepartments"] = new SelectList(departments, selectedDepartment);
        ViewData["ApproverUsers"] = approverUsers;
        ViewData["SelectedApproverUserId"] = selectedApproverId;
    }

    private async Task PopulateDriveAccessDepartmentSelectionsAsync(string? selectedDepartment)
    {
        var departments = await _context.Users
            .AsNoTracking()
            .Select(user => user.Department)
            .Where(department => !string.IsNullOrWhiteSpace(department))
            .Select(department => department!.Trim())
            .Distinct()
            .OrderBy(department => department)
            .ToListAsync();

        ViewData["DriveAccessDepartments"] = new SelectList(departments, selectedDepartment);
    }

    private async Task PopulateItSupportSelectionsAsync(string? selectedItUserId)
    {
        var itSupportUsers = await GetItSupportUsersAsync();
        ViewData["ItSupportUsers"] = itSupportUsers;
        ViewData["SelectedItSupportUserId"] = selectedItUserId;
    }

    private static string GetActorName(ApplicationUser? user)
    {
        if (!string.IsNullOrWhiteSpace(user?.FullName))
        {
            return user.FullName;
        }

        return user?.UserName ?? user?.Email ?? "System";
    }

    private static bool IsOwnerTicket(RepairTicket ticket, ApplicationUser? user)
    {
        if (ticket.RequesterUserId == user?.Id)
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(ticket.RequesterUserId))
        {
            return false;
        }

        var requesterName = ticket.RequesterName?.Trim();
        var createdByName = ticket.CreatedByName?.Trim();

        var candidates = new[]
        {
            user?.FullName,
            user?.UserName,
            user?.Email
        };

        return candidates.Any(candidate =>
            !string.IsNullOrWhiteSpace(candidate)
            && (
                (!string.IsNullOrWhiteSpace(requesterName)
                    && string.Equals(requesterName, candidate.Trim(), StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(createdByName)
                    && string.Equals(createdByName, candidate.Trim(), StringComparison.OrdinalIgnoreCase))
            ));
    }

    private async Task<string> GenerateDocumentNo(DateTime utcNow)
    {
        var thaiTime = utcNow.AddHours(7);
        var year = thaiTime.ToString("yy");
        var month = thaiTime.Month;
        var monthCode = month switch
        {
            10 => "A",
            11 => "B",
            12 => "C",
            _ => month.ToString()
        };

        var startOfMonthThai = new DateTime(thaiTime.Year, thaiTime.Month, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var startOfMonthUtc = startOfMonthThai.AddHours(-7);
        var endOfMonthUtc = startOfMonthUtc.AddMonths(1);

        var requestsInMonth = await _context.RepairTickets
            .CountAsync(ticket => ticket.CreatedAt >= startOfMonthUtc && ticket.CreatedAt < endOfMonthUtc);
        var runningNumber = requestsInMonth + 1;

        return $"SR-{year}{monthCode}-{runningNumber:D3}";
    }

    private void AddStatusHistory(
        RepairTicket ticket,
        TicketStatus? fromStatus,
        TicketStatus toStatus,
        ApplicationUser? actor,
        string action,
        string? remark = null)
    {
        _context.AddRepairTicketStatusHistory(
            ticket,
            fromStatus,
            toStatus,
            action,
            remark,
            actor?.Id,
            GetActorName(actor));
    }

    private static List<TicketStatus> GetEditableStatusFlow(List<TicketStatus> statusFlow)
    {
        var editableFlow = statusFlow.Count > 0
            ? new List<TicketStatus>(statusFlow)
            : new List<TicketStatus> { TicketStatus.Open };

        if (editableFlow.Count > 1 && editableFlow[0] == TicketStatus.Open)
        {
            editableFlow.RemoveAt(0);
        }

        return editableFlow;
    }

    private static List<TicketStatus> CollapseConsecutiveStatuses(IEnumerable<TicketStatus> statuses)
    {
        var collapsed = new List<TicketStatus>();

        foreach (var status in statuses)
        {
            if (collapsed.Count == 0 || collapsed[^1] != status)
            {
                collapsed.Add(status);
            }
        }

        return collapsed;
    }
}
