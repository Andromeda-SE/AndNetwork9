using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace And9.Integration.Discord.Database;

public class DesignDiscordDataContext : IDesignTimeDbContextFactory<DiscordDataContext>
{
    public DiscordDataContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<DiscordDataContext> builder = new();
        string connectionString = args.Any()
            ? string.Join(' ', args)
            : "User ID=postgres;Password=postgres;Host=localhost;Port=5432;";
        Console.WriteLine(@"connectionString = " + connectionString);
        return new(builder
            .UseNpgsql(connectionString)
            .Options);
    }
}