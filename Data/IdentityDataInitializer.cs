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
            // Seed Roles: Admin and User
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }

        private static async Task SeedUsers(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            // Always remove existing admin user (if any)
            var existingAdmin = await userManager.FindByNameAsync("ituser@example.com");
            if (existingAdmin != null)
            {
                await userManager.DeleteAsync(existingAdmin);
            }

            // Generate a random password
            var random = new Random();
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
            string password = new string(Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)]).ToArray());

            // Create the Admin User
            var adminUser = new ApplicationUser
            {
                UserName = "ituser@example.com",
                Email = "ituser@example.com",
                FirstName = "Admin",
                LastName = "User",
                Department = "IT",
                Section = "Support",
                Plant = "Headquarters",
                IsActive = true,
                IsIT = true,
                EmailConfirmed = true // Bypassing email confirmation for the seed user
            };

            var result = await userManager.CreateAsync(adminUser, password);

            if (result.Succeeded)
            {
                // Log the password to the console, DEBUG output, and database (AdminPasswordLog table if exists)
                Console.WriteLine($"[ADMIN SEED] Username: ituser@example.com Password: {password}");
                System.Diagnostics.Debug.WriteLine($"[ADMIN SEED] Username: ituser@example.com Password: {password}");
                try
                {
                    if (context.Database.CanConnect())
                    {
                        // Commented out CREATE TABLE for cross-db compatibility
                        // context.Database.ExecuteSqlRaw($"IF OBJECT_ID('AdminPasswordLog', 'U') IS NULL CREATE TABLE AdminPasswordLog (Id INT IDENTITY(1,1) PRIMARY KEY, Username NVARCHAR(256), Password NVARCHAR(256), CreatedAt DATETIME)");
                        try {
                            context.Database.ExecuteSqlRaw(
                                "INSERT INTO AdminPasswordLog (Username, Password, CreatedAt) VALUES (@username, @password, GETDATE())",
                                new[] {
                                    new Microsoft.Data.SqlClient.SqlParameter("@username", "ituser@example.com"),
                                    new Microsoft.Data.SqlClient.SqlParameter("@password", password ?? string.Empty)
                                });
                        } catch { /* ignore errors for log table */ }
                    }
                }
                catch { /* ignore errors for log table */ }
                // Assign the 'Admin' role
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
