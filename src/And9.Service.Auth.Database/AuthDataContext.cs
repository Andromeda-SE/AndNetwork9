using And9.Service.Auth.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Auth.Database;

public class AuthDataContext : DbContext
{
    public AuthDataContext(DbContextOptions<AuthDataContext> options) : base(options) { }

    public DbSet<AccessRule> AccessRules { get; set; } = null!;
    public DbSet<PasswordHash> PasswordHashes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Auth");

        modelBuilder.Entity<AccessRule>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.MinRank).IsRequired();
            entity.Property(x => x.Directions).IsRequired();
            entity.Property(x => x.SquadNumber);
            entity.Property(x => x.SquadPartNumber);
            entity.Property(x => x.AllowedMembersIds).IsRequired();

            entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<PasswordHash>(entity =>
        {
            entity.HasKey(x => x.UserId);

            entity.Property(x => x.Hash).IsRequired();
        });
    }
}