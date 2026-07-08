namespace ITRepairService.ViewModels.AdminUsers;

public class AdminUserListItemViewModel
{
    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public IList<string> Roles { get; set; } = [];
}
