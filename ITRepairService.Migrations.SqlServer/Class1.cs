using ITRepairService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace ITRepairService.Migrations.SqlServer;

public class SqlServerAppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var migrationsAssembly = Assembly.GetExecutingAssembly().GetName().Name;
        builder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=ITRepairServiceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
            options => options.MigrationsAssembly(migrationsAssembly));

        return new AppDbContext(builder.Options);
    }
}
