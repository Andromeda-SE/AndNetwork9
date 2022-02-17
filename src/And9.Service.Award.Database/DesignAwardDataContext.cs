using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace And9.Service.Award.Database;

public class DesignAwardDataContext : IDesignTimeDbContextFactory<AwardDataContext>
{
    public AwardDataContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<AwardDataContext> builder = new();
        string connectionString = args.Any()
            ? string.Join(' ', args)
            : "User ID=postgres;Password=postgres;Host=localhost;Port=5432;";
        Console.WriteLine(@"connectionString = " + connectionString);
        return new(builder
            .UseNpgsql(connectionString)
            .Options);
    }
}