using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace And9.Service.Core.Database;

public class DesignCoreDataContext : IDesignTimeDbContextFactory<CoreDataContext>
{
    public CoreDataContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<CoreDataContext> builder = new();
        string connectionString = args.Any()
            ? string.Join(' ', args)
            : "User ID=postgres;Password=postgres;Host=localhost;Port=5432;";
        Console.WriteLine(@"connectionString = " + connectionString);
        return new(builder
            .UseNpgsql(connectionString)
            .Options);
    }
}