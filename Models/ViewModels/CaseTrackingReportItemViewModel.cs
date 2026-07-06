using System;

namespace myapp.Models.ViewModels
{
    public class CaseTrackingReportItemViewModel
    {
        public int RequestId { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string Requester { get; set; } = string.Empty;
        public string? RequesterDepartment { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AssignedToName { get; set; } = "-";
        public DateTime RequestDate { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public double WorkingHours { get; set; }
        public string WorkingDurationText { get; set; } = "-";
    }
}
