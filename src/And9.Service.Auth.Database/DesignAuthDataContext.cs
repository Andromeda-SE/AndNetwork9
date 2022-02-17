using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace And9.Service.Auth.Database;

public class DesignAuthDataContext : IDesignTimeDbContextFactory<AuthDataContext>
{
    public AuthDataContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<AuthDataContext> builder = new();
        string connectionString = args.Any()
            ? string.Join(' ', args)
            : "User ID=postgres;Password=postgres;Host=localhost;Port=5432;";
        Console.WriteLine(@"connectionString = " + connectionString);
        return new(builder
            .UseNpgsql(connectionString)
            .Options);
    }
}