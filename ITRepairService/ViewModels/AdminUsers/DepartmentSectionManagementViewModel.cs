using System.ComponentModel.DataAnnotations;

namespace ITRepairService.ViewModels.AdminUsers;

public class DepartmentSectionManagementViewModel
{
    public IReadOnlyList<DepartmentUsageViewModel> Departments { get; init; } = Array.Empty<DepartmentUsageViewModel>();
    public IReadOnlyList<SectionUsageViewModel> Sections { get; init; } = Array.Empty<SectionUsageViewModel>();
    public IReadOnlyList<string> AvailableDepartments { get; init; } = Array.Empty<string>();

    public CreateDepartmentInputModel CreateDepartment { get; init; } = new();
    public CreateSectionInputModel CreateSection { get; init; } = new();
    public RenameDepartmentInputModel RenameDepartment { get; init; } = new();
    public RenameSectionInputModel RenameSection { get; init; } = new();
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

public class SectionUsageViewModel
{
    public string Name { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public int UserCount { get; init; }
    public bool IsActive { get; init; } = true;
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

public class RenameSectionInputModel
{
    [Required]
    [Display(Name = "Section เดิม")]
    public string OldSection { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Department")]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    public IReadOnlyList<string> AvailableDepartments { get; set; } = Array.Empty<string>();

    [Required]
    [Display(Name = "Section ใหม่")]
    [StringLength(100)]
    public string NewSection { get; set; } = string.Empty;
}

public class CreateSectionInputModel
{
    [Required]
    [Display(Name = "Department")]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Section ใหม่")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}
