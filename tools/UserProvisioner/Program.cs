using ITRepairService.Data;
using ITRepairService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

const string targetUserName = "ASI006038";
const string targetEmail = "ASI006038@stanley-electic.com";
const string defaultPassword = "Asi006038@";

var services = new ServiceCollection();
services.AddLogging();
services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=ITRepairService/itrepair.db"));
services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

using var provider = services.BuildServiceProvider();
var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

var ownerByName = await userManager.FindByNameAsync(targetUserName);
var ownerByEmail = await userManager.FindByEmailAsync(targetEmail);
Console.WriteLine($"Lookup by username '{targetUserName}': {(ownerByName is null ? "not found" : ownerByName.Email)}");
Console.WriteLine($"Lookup by email '{targetEmail}': {(ownerByEmail is null ? "not found" : ownerByEmail.UserName)}");

if (!await roleManager.RoleExistsAsync(AppRoles.User))
{
    await roleManager.CreateAsync(new IdentityRole(AppRoles.User));
}

var existingByEmail = await userManager.FindByEmailAsync(targetEmail);
if (existingByEmail is not null)
{
    var existingByName = await userManager.FindByNameAsync(targetUserName);
    if (existingByName is not null && existingByName.Id != existingByEmail.Id)
    {
        Console.WriteLine($"FAILED: Username '{targetUserName}' is already used by another account.");
        return;
    }

    var userNameResult = await userManager.SetUserNameAsync(existingByEmail, targetUserName);
    if (!userNameResult.Succeeded)
    {
        Console.WriteLine("FAILED: Could not set username.");
        foreach (var error in userNameResult.Errors)
        {
            Console.WriteLine(error.Description);
        }
        return;
    }

    if (string.IsNullOrWhiteSpace(existingByEmail.FullName))
    {
        existingByEmail.FullName = targetUserName;
    }

    if (string.IsNullOrWhiteSpace(existingByEmail.Department))
    {
        existingByEmail.Department = "IT";
    }

    if (string.IsNullOrWhiteSpace(existingByEmail.Section))
    {
        existingByEmail.Section = "Helpdesk";
    }

    existingByEmail.IsActive = true;
    existingByEmail.UpdatedAt = DateTime.UtcNow;
    existingByEmail.UpdatedByName = "System";

    var updateResult = await userManager.UpdateAsync(existingByEmail);
    if (!updateResult.Succeeded)
    {
        Console.WriteLine("FAILED: Could not update user.");
        foreach (var error in updateResult.Errors)
        {
            Console.WriteLine(error.Description);
        }
        return;
    }

    if (!await userManager.IsInRoleAsync(existingByEmail, AppRoles.User))
    {
        await userManager.AddToRoleAsync(existingByEmail, AppRoles.User);
    }

    Console.WriteLine($"UPDATED: {targetEmail} now has username '{targetUserName}'.");
    return;
}

var newUser = new ApplicationUser
{
    UserName = targetUserName,
    Email = targetEmail,
    FullName = targetUserName,
    Department = "IT",
    Section = "Helpdesk",
    IsActive = true,
    EmailConfirmed = true,
    CreatedByName = "System",
    UpdatedByName = "System",
    CreatedAt = DateTime.UtcNow,
    UpdatedAt = DateTime.UtcNow
};

var createResult = await userManager.CreateAsync(newUser, defaultPassword);
if (!createResult.Succeeded)
{
    Console.WriteLine("FAILED: Could not create user.");
    foreach (var error in createResult.Errors)
    {
        Console.WriteLine(error.Description);
    }
    return;
}

await userManager.AddToRoleAsync(newUser, AppRoles.User);
Console.WriteLine($"CREATED: Username '{targetUserName}' with email '{targetEmail}'.");
Console.WriteLine($"DEFAULT PASSWORD: {defaultPassword}");
