using And9.Service.Core.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.Database;

public class CoreDataContext : DbContext
{
    public CoreDataContext(DbContextOptions<CoreDataContext> options) : base(options) { }

    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<CandidateRegisteredRequest> CandidateRequests { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Core");

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.DiscordId).IsUnique();
            entity.Property(x => x.DiscordId).IsRequired(false);
            entity.HasIndex(x => x.SteamId).IsUnique();
            entity.Property(x => x.SteamId).IsRequired(false);
            entity.HasIndex(x => x.MicrosoftId).IsUnique();
            entity.Property(x => x.MicrosoftId).IsRequired(false);
            entity.HasIndex(x => x.VkId).IsUnique();
            entity.Property(x => x.VkId).IsRequired(false);
            entity.HasIndex(x => x.TelegramId).IsUnique();
            entity.Property(x => x.TelegramId).IsRequired(false);

            entity.HasIndex(x => x.Nickname).IsUnique();
            entity.Property(x => x.Nickname).IsRequired();
            entity.Property(x => x.RealName);

            entity.HasIndex(x => x.Rank).IsUnique(false);
            entity.Property(x => x.Rank);
            entity.Ignore(x => x.RankIcon);
            entity.HasIndex(x => x.Direction).IsUnique(false);
            entity.Property(x => x.Direction);
            entity.Property(x => x.SquadNumber).IsRequired(false);
            entity.HasIndex(x => x.SquadNumber).IsUnique(false);
            entity.Property(x => x.SquadPartNumber).IsRequired();
            entity.HasIndex(x => new {x.SquadNumber, x.SquadPartNumber}).IsUnique(false);

            entity.Property(x => x.TimeZone).HasConversion(x => x!.Id, x => TimeZoneInfo.FindSystemTimeZoneById(x)).IsRequired(false);
            entity.Property(x => x.JoinDate);
            entity.Property(x => x.LastDirectionChange);

            entity.Property(x => x.ConcurrencyToken).IsConcurrencyToken();
            entity.Property(x => x.LastChanged).IsRowVersion();
        });

        modelBuilder.Entity<CandidateRegisteredRequest>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Member).WithMany().HasForeignKey(x => x.MemberId).IsRequired();

            entity.Property(x => x.HoursCount);
            entity.Property(x => x.Age);
            entity.Property(x => x.Recommendation);
            entity.Property(x => x.Description);
            entity.HasIndex(x => x.AuxiliarySquad).IsUnique(false);
            entity.Property(x => x.AuxiliarySquad);
            entity.HasIndex(x => x.Accepted).IsUnique(false);
            entity.Property(x => x.Accepted);

            entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
        });
    }
}