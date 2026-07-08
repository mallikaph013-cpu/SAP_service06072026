using ITRepairService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace ITRepairService.Migrations.Sqlite;

public class SqliteAppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var baseDir = Directory.GetCurrentDirectory();
        var projectDir = Path.Combine(baseDir, "..", "ITRepairService");
        var dbPath = Path.Combine(projectDir, "itrepair.db");

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var migrationsAssembly = Assembly.GetExecutingAssembly().GetName().Name;
        builder.UseSqlite(
            $"Data Source={dbPath}",
            options => options.MigrationsAssembly(migrationsAssembly));

        return new AppDbContext(builder.Options);
    }
}
