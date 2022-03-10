using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace And9.Service.Election.Database;

public class DesignElectionDataContext : IDesignTimeDbContextFactory<ElectionDataContext>
{
    public ElectionDataContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ElectionDataContext> builder = new();
        string connectionString = args.Any()
            ? string.Join(' ', args)
            : "User ID=postgres;Password=postgres;Host=localhost;Port=5432;";
        Console.WriteLine(@"connectionString = " + connectionString);
        return new(builder
            .UseNpgsql(connectionString)
            .Options);
    }
}