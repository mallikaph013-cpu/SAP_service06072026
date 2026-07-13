using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ITRepairService.ViewModels.AdminUsers;

public class DepartmentSectionManagementViewModel
{
    public IReadOnlyList<DepartmentUsageViewModel> Departments { get; init; } = Array.Empty<DepartmentUsageViewModel>();
    public IReadOnlyList<string> AvailableDepartments { get; init; } = Array.Empty<string>();

    public CreateDepartmentInputModel CreateDepartment { get; init; } = new();
    public RenameDepartmentInputModel RenameDepartment { get; init; } = new();
}

public class DepartmentUsageViewModel
{
    public string Name { get; init; } = string.Empty;
    public int UserCount { get; init; }
    public int RequesterTicketCount { get; init; }
    public int ApproverTicketCount { get; init; }
    public bool IsActive { get; init; } = true;

    public int TotalUsageCount => UserCount + RequesterTicketCount + ApproverTicketCount;
}

public class RenameDepartmentInputModel
{
    [Required]
    [Display(Name = "Department เดิม")]
    public string OldDepartment { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Department ใหม่")]
    [StringLength(100)]
    public string NewDepartment { get; set; } = string.Empty;
}

public class CreateDepartmentInputModel
{
    [Required]
    [Display(Name = "Department ใหม่")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}

