namespace ITRepairService.Models;

public static class AppRoles
{
    public const string User = "User";
    public const string ITSupport = "ITSupport";
    public const string Approve = "Approve";
    public const string Admin = "Admin";

    public static readonly string[] All = [User, ITSupport, Approve, Admin];
}
