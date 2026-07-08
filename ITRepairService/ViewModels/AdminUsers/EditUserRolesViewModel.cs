using System.ComponentModel.DataAnnotations;

namespace ITRepairService.ViewModels.AdminUsers;

public class EditUserRolesViewModel
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Department")]
    public string Department { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Section")]
    public string Section { get; set; } = string.Empty;

    [Display(Name = "Status")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Force change password on next login")]
    public bool MustChangePassword { get; set; } = false;

    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string? ConfirmPassword { get; set; }

    public IList<string> AvailableRoles { get; set; } = [];

    public IList<string> SelectedRoles { get; set; } = [];

    public IList<string> AvailableDepartments { get; set; } = [];

    public IList<string> AvailableSections { get; set; } = [];

    public IList<DepartmentSectionOptionViewModel> SectionDepartmentOptions { get; set; } = [];
}
