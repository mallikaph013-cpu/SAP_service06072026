using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ITRepairService.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public bool MustChangePassword { get; set; } = false;

    [StringLength(120)]
    public string? CreatedByName { get; set; }

    [StringLength(120)]
    public string? UpdatedByName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
