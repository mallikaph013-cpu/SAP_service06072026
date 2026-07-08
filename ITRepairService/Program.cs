using ITRepairService.Data;
using ITRepairService.Models;
using ITRepairService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "SQLite";
var sqliteConnection = builder.Configuration.GetConnectionString("SQLite")
    ?? builder.Configuration.GetConnectionString("SqliteConnection")
    ?? throw new InvalidOperationException("Connection string 'SQLite' is missing.");
var sqlServerConnection = builder.Configuration.GetConnectionString("SQLServer")
    ?? builder.Configuration.GetConnectionString("SqlServerConnection")
    ?? throw new InvalidOperationException("Connection string 'SQLServer' is missing.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(sqlServerConnection, sqlServerOptions =>
            sqlServerOptions.MigrationsAssembly("ITRepairService.Migrations.SqlServer"));
    }
    else
    {
        options.UseSqlite(sqliteConnection, sqliteOptions =>
            sqliteOptions.MigrationsAssembly("ITRepairService.Migrations.Sqlite"));
        options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }
});

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Register LDAP authentication service
builder.Services.Configure<LdapSettings>(builder.Configuration.GetSection("LDAP"));
builder.Services.AddSingleton<ILdapAuthenticationService, LdapAuthenticationService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

await IdentitySeeder.SeedRolesAndAdminAsync(app.Services, app.Configuration);

app.Run();

