using System.Drawing;
using And9.Integration.Discord.Abstractions.Models;
using And9.Integration.Discord.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace And9.Integration.Discord.Database;

public class DiscordDataContext : DbContext
{
    public DiscordDataContext(DbContextOptions<DiscordDataContext> options) : base(options) { }

    public DbSet<ChannelCategory> ChannelCategories { get; set; } = null!;
    public DbSet<Channel> Channels { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Discord");

        modelBuilder.Entity<ChannelCategory>(entity =>
        {
            entity.HasKey(x => x.Position);
            entity.HasAlternateKey(x => x.DiscordId);

            entity.Property(x => x.Name).IsRequired();

            entity.Property(x => x.ConcurrencyToken).IsConcurrencyToken();
            entity.Property(x => x.LastChanged).IsRowVersion();

            entity.HasMany(x => x.Channels).WithOne(x => x.Category).HasForeignKey(x => x.CategoryId);
        });

        modelBuilder.Entity<Channel>(entity =>
        {
            entity.HasKey(x => x.DiscordId);

            entity.HasIndex(x => x.ChannelPosition).IsUnique(false);
            entity.Property(x => x.ChannelPosition).IsRequired();
            entity.Property(x => x.Name).IsRequired();
            entity.HasIndex(x => x.Direction).IsUnique(false);
            entity.Property(x => x.Direction);
            entity.HasIndex(x => x.SquadNumber).IsUnique(false);
            entity.Property(x => x.SquadNumber);
            entity.HasIndex(x => new {x.SquadNumber, x.SquadPartNumber}).IsUnique(false);
            entity.Property(x => x.SquadPartNumber);
            entity.Property(x => x.EveryonePermissions).IsRequired();
            entity.Property(x => x.MemberPermissions).IsRequired();
            entity.Property(x => x.SquadPartPermissions).IsRequired();
            entity.Property(x => x.SquadPartCommanderPermissions).IsRequired();
            entity.Property(x => x.SquadPermissions).IsRequired();
            entity.Property(x => x.SquadLieutenantsPermissions).IsRequired();
            entity.Property(x => x.SquadCaptainPermissions).IsRequired();
            entity.Property(x => x.AdvisorPermissions).IsRequired();

            entity.Property(x => x.ConcurrencyToken).IsConcurrencyToken();
            entity.Property(x => x.LastChanged).IsRowVersion();

            entity.HasIndex(x => new {x.CategoryId, x.ChannelPosition}).IsUnique();
            entity.HasOne(x => x.Category).WithMany(x => x.Channels).HasForeignKey(x => x.CategoryId);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(x => x.DiscordId);

            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.Color).HasConversion(
                x => x.HasValue ? x.Value.ToArgb() : default(int?),
                i => i.HasValue ? Color.FromArgb(i.Value) : default(Color?),
                ValueComparer.CreateDefault(typeof(Color?), false));
            entity.Property(x => x.Direction);
            entity.Property(x => x.SquadNumber);
            entity.Property(x => x.SquadPartNumber);
            entity.Property(x => x.GlobalPermissions).IsRequired();
            entity.Property(x => x.Scope).IsRequired();

            entity.Property(x => x.ConcurrencyToken).IsConcurrencyToken();
            entity.Property(x => x.LastChanged).IsRowVersion();
        });
    }
}