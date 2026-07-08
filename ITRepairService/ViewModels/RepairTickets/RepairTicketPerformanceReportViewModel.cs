namespace ITRepairService.ViewModels.RepairTickets;

public class RepairTicketPerformanceReportViewModel
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string ItName { get; set; } = string.Empty;

    public List<RepairTicketPerformanceCaseViewModel> Cases { get; set; } = new();

    public List<RepairTicketPerformanceStaffSummaryViewModel> StaffSummaries { get; set; } = new();
}

public class RepairTicketPerformanceCaseViewModel
{
    public int TicketId { get; set; }

    public string DocumentNo { get; set; } = string.Empty;

    public string RequesterName { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string AssignedItName { get; set; } = string.Empty;

    public DateTime AssignedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public ITRepairService.Models.TicketStatus CurrentStatus { get; set; }

    public TimeSpan Duration { get; set; }
}

public class RepairTicketPerformanceStaffSummaryViewModel
{
    public string ItStaffName { get; set; } = string.Empty;

    public int ClosedCases { get; set; }

    public double AverageHours { get; set; }

    public double MinimumHours { get; set; }

    public double MaximumHours { get; set; }
}
