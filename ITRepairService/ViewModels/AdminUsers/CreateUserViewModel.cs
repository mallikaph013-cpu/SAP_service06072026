using System.ComponentModel.DataAnnotations;

namespace ITRepairService.ViewModels.AdminUsers;

public class CreateUserViewModel
{
    [Required]
    [Display(Name = "Username")]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Username may only contain letters, numbers, dots, hyphens, and underscores.")]
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

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Roles")]
    public List<string> SelectedRoles { get; set; } = new();

    public List<string> AvailableRoles { get; set; } = new();
    public List<string> AvailableDepartments { get; set; } = new();
    public List<string> AvailableSections { get; set; } = new();
    public List<DepartmentSectionOptionViewModel> SectionDepartmentOptions { get; set; } = new();
}

public class DepartmentSectionOptionViewModel
{
    public string Department { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
}
