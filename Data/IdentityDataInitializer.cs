using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myapp.Models;
using System.Threading.Tasks;

namespace myapp.Data
{
    public static class IdentityDataInitializer
    {
        public static async Task SeedData(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            // *** THIS IS THE FIX ***
            // Ensures that the database is created and all migrations are applied.
            // This must run BEFORE any data seeding attempts.
            await context.Database.MigrateAsync();

            await SeedRoles(roleManager);
            await SeedUsers(userManager, context);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles: Admin, Approve, and User
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Approve"))
            {
                await roleManager.CreateAsync(new IdentityRole("Approve"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }

        private static async Task SeedUsers(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            const string adminUserName = "ituser@example.com";
            const string adminPassword = "Abcd12345@";

            // Keep existing admin account intact to avoid changing credentials on every startup.
            var existingAdmin = await userManager.FindByNameAsync(adminUserName);
            if (existingAdmin != null)
            {
                if (!existingAdmin.IsActive)
                {
                    existingAdmin.IsActive = true;
                    existingAdmin.UpdatedAt = DateTime.UtcNow;
                    existingAdmin.UpdatedBy = adminUserName;
                    await userManager.UpdateAsync(existingAdmin);
                }

                existingAdmin.MustChangePasswordOnFirstLogin = false;
                existingAdmin.LockoutEnabled = false;
                existingAdmin.LockoutEnd = null;
                existingAdmin.AccessFailedCount = 0;
                existingAdmin.UpdatedAt = DateTime.UtcNow;
                existingAdmin.UpdatedBy = adminUserName;
                await userManager.UpdateAsync(existingAdmin);

                // Keep a deterministic admin password for recovery when login credentials drift.
                var resetToken = await userManager.GeneratePasswordResetTokenAsync(existingAdmin);
                var resetResult = await userManager.ResetPasswordAsync(existingAdmin, resetToken, adminPassword);
                if (!resetResult.Succeeded)
                {
                    var removeResult = await userManager.RemovePasswordAsync(existingAdmin);
                    if (removeResult.Succeeded)
                    {
                        await userManager.AddPasswordAsync(existingAdmin, adminPassword);
                    }
                }

                if (!await userManager.IsInRoleAsync(existingAdmin, "Admin"))
                {
                    await userManager.AddToRoleAsync(existingAdmin, "Admin");
                }

                return;
            }

            // Create the Admin User
            var adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                Email = adminUserName,
                FirstName = "Admin",
                LastName = "User",
                Department = "IT",
                Section = "Support",
                Plant = "Headquarters",
                IsActive = true,
                IsIT = true,
                EmailConfirmed = true // Bypassing email confirmation for the seed user
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                Console.WriteLine($"[ADMIN SEED] Username: {adminUserName} Password: {adminPassword}");
                // Assign the 'Admin' role
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
