using System.ComponentModel.DataAnnotations;

namespace ITRepairService.ViewModels.Account;

public class ChangePasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "รหัสผ่านปัจจุบัน")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "{0} ต้องมีความยาวอย่างน้อย {2} และไม่เกิน {1} ตัวอักษร", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "รหัสผ่านใหม่")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "ยืนยันรหัสผ่านใหม่")]
    [Compare("NewPassword", ErrorMessage = "รหัสผ่านใหม่และยืนยันรหัสผ่านไม่ตรงกัน")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
