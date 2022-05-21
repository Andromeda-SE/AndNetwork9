using And9.Service.Core.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core.Database;

public class CoreDataContext : DbContext
{
    public CoreDataContext(DbContextOptions<CoreDataContext> options) : base(options) { }

    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<CandidateRegisteredRequest> CandidateRequests { get; set; } = null!;
    public DbSet<Specialization> Specializations { get; set; } = null!;
    public DbSet<MemberSpecialization> MemberSpecializations { get; set; } = null!;
    public DbSet<Squad?> Squads { get; set; } = null!;
    public DbSet<SquadMembershipHistoryEntry> SquadMembershipHistory { get; set; } = null!;
    public DbSet<SquadRequest> SquadRequests { get; set; } = null!;

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
            entity.Property(x => x.SquadNumber).IsRequired(false);
            entity.HasIndex(x => x.SquadNumber).IsUnique(false);
            entity.Property(x => x.SquadPartNumber).IsRequired();
            entity.HasIndex(x => new {x.SquadNumber, x.SquadPartNumber}).IsUnique(false);

            entity.Property(x => x.TimeZone).HasConversion(x => x!.Id, x => TimeZoneInfo.FindSystemTimeZoneById(x)).IsRequired(false);
            entity.Property(x => x.JoinDate);

            entity.HasMany(x => x.SquadMembershipHistoryEntries).WithOne(x => x.Member).HasForeignKey(x => x.MemberId);
            entity.HasMany(x => x.Specializations).WithOne(x => x.Member).HasForeignKey(x => x.MemberId);
            entity.HasOne(x => x.Squad).WithMany(x => x.Members).HasForeignKey(x => x.SquadNumber).IsRequired(false);
            entity.HasMany(x => x.SquadRequests).WithOne(x => x.Member).HasForeignKey(x => x.MemberId);

            entity.Property(x => x.ConcurrencyToken).IsConcurrencyToken().HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<CandidateRegisteredRequest>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Member).WithMany().HasForeignKey(x => x.MemberId).IsRequired();

            entity.Property(x => x.HoursCount);
            entity.Property(x => x.Age);
            entity.Property(x => x.Recommendation);
            entity.Property(x => x.Description).IsRequired();
            entity.HasIndex(x => x.AuxiliarySquad).IsUnique(false);
            entity.Property(x => x.AuxiliarySquad);
            entity.HasIndex(x => x.Accepted).IsUnique(false);
            entity.Property(x => x.Accepted);

            entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Direction);
            entity.Property(x => x.Name);
            entity.Property(x => x.Description);
            entity.HasIndex(x => x.Direction).IsUnique(false);
            entity.HasIndex(x => new {x.Direction, x.Name}).IsUnique();

            entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");

            entity.HasData(SpecializationsData.GetSpecializations());
        });

        modelBuilder.Entity<MemberSpecialization>(entity =>
        {
            entity.HasKey(x => new {x.MemberId, x.SpecializationId});

            entity.HasOne(x => x.Member).WithMany(x => x.Specializations).HasForeignKey(x => x.MemberId);
            entity.HasIndex(x => x.MemberId).IsUnique(false);
            entity.HasOne(x => x.Specialization).WithMany(x => x.MemberSpecializations).HasForeignKey(x => x.SpecializationId);
            entity.Property(x => x.Priority).IsRequired(false);
            entity.HasIndex(x => new {x.MemberId, x.Priority}).IsUnique();
            entity.Property(x => x.ApproveDateTime).IsRequired(false);

            entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<Squad>(entity =>
        {
            entity.HasKey(x => x.Number);

            entity.Property(x => x.Names);
            entity.Property(x => x.CreateDate).IsRequired().ValueGeneratedOnAdd().HasDefaultValueSql("now()");
            entity.Property(x => x.IsActive).IsRequired();
            entity.HasIndex(x => x.IsActive).IsUnique(false);

            entity.HasMany(x => x.SquadMembershipHistoryEntries).WithOne(x => x.Squad).HasForeignKey(x => x.SquadId);
            entity.HasMany(x => x.Members).WithOne(x => x.Squad).HasForeignKey(x => x.SquadNumber);
            entity.HasMany(x => x.SquadRequests).WithOne(x => x.Squad).HasForeignKey(x => x.MemberId);

            entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<SquadMembershipHistoryEntry>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Member).WithMany(x => x.SquadMembershipHistoryEntries).HasForeignKey(x => x.MemberId);
            entity.HasOne(x => x.Squad).WithMany(x => x.SquadMembershipHistoryEntries).HasForeignKey(x => x.SquadId);

            entity.Property(x => x.JoinDateTime);
            entity.Property(x => x.LeaveDateTime);

            entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<SquadRequest>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Member).WithMany(x => x.SquadRequests).HasForeignKey(x => x.MemberId);
            entity.HasIndex(x => x.MemberId);
            entity.HasOne(x => x.Squad).WithMany(x => x.SquadRequests).HasForeignKey(x => x.SquadNumber);
            entity.HasIndex(x => x.SquadNumber);
            entity.Property(x => x.Accepted).IsRequired(false);
            entity.HasIndex(x => x.Accepted).IsUnique(false);
            entity.Property(x => x.IsCanceledByMember).IsRequired();
            entity.Property(x => x.CreateDateTime).IsRequired().ValueGeneratedOnAdd().HasDefaultValueSql("now()");

            entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
        });
    }
}