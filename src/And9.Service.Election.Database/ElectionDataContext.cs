using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Election.Database;

public class ElectionDataContext : DbContext
{
    public ElectionDataContext(DbContextOptions<ElectionDataContext> options) : base(options) { }

    public DbSet<Abstractions.Models.Election> Elections { get; set; } = null!;
    public DbSet<ElectionVote> ElectionVotes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Election");

        modelBuilder.HasSequence<short>("ElectionIds");
        modelBuilder.Entity<Abstractions.Models.Election>(entity =>
        {
            entity.HasKey(x => new
            {
                x.ElectionId,
                x.Direction,
            });
            entity.Property(x => x.ElectionId).HasDefaultValueSql("nextval('\"Election\".\"ElectionIds\"')");
            entity.HasIndex(x => x.ElectionId).IsUnique(false);
            entity.Property(x => x.Direction);
            entity.HasIndex(x => x.Direction).IsUnique(false);

            entity.Property(x => x.AdvisorsStartDate);
            entity.Property(x => x.Status);
            entity.Property(x => x.AgainstAllVotes);

            entity.Property(x => x.ConcurrencyToken).IsConcurrencyToken();
            entity.Property(x => x.LastChanged).IsRowVersion();
        });
        
        modelBuilder.Entity<ElectionVote>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasAlternateKey(x => new
            {
                x.ElectionId,
                x.Direction,
                x.MemberId
            });

            entity.Property(x => x.ElectionId);
            entity.HasIndex(x => x.ElectionId).IsUnique(false);
            entity.Property(x => x.Direction);
            entity.HasIndex(x => x.Direction).IsUnique(false);
            entity.HasIndex(x => new
            {
                x.ElectionId,
                x.Direction,
            }).IsUnique(false);

            entity.Property(x => x.MemberId);
            entity.Property(x => x.Voted).IsRequired(false);
            entity.Property(x => x.Votes);

            entity.Property(x => x.ConcurrencyToken).IsConcurrencyToken();
            entity.Property(x => x.LastChanged).IsRowVersion();
        });
    }

    public async ValueTask<Abstractions.Models.Election> GetCurrentElectionAsync() => 
        await Elections.SingleAsync(x => x.Status > ElectionStatus.None && x.Status < ElectionStatus.Ended).ConfigureAwait(false);
}