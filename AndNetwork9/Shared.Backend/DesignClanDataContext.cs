using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AndNetwork9.Shared.Backend
{
    // ReSharper disable once UnusedMember.Global
    public class DesignClanDataContext : IDesignTimeDbContextFactory<ClanDataContext>
    {
        public ClanDataContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ClanDataContext> builder = new();
            string connectionString = args.Any()
                ? string.Join(' ', args)
                : "User ID=postgres;Password=postgres;Host=localhost;Port=5432;";
            Console.WriteLine("connectionString = " + connectionString);
            return new(builder
                .UseNpgsql(connectionString)
                .Options);
        }
    }
}