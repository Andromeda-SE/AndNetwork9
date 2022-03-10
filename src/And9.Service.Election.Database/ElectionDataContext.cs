using And9.Service.Core.Abstractions.Enums;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Abstractions.Models;
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

        modelBuilder.Entity<Abstractions.Models.Election>(entity =>
        {
            entity.HasKey(x => x.ElectionId);
            entity.Property(x => x.Direction);
            entity.HasIndex(x => x.Direction).IsUnique(false);
            entity.HasAlternateKey(x => new
            {
                x.ElectionId,
                x.Direction,
            });
            entity.Property(x => x.AdvisorsStartDate);
            entity.Property(x => x.Status);
            entity.Property(x => x.AgainstAllVotes);

            entity.HasMany(x => x.Votes).WithOne().HasForeignKey(x => x.ElectionId);

            entity.Property(x => x.ConcurrencyToken).IsConcurrencyToken().HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<ElectionVote>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasAlternateKey(x => new
            {
                x.ElectionId,
                x.Direction,
                x.MemberId,
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

            entity.Property(x => x.ConcurrencyToken).IsConcurrencyToken().HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
        });
    }

    public IAsyncEnumerable<Abstractions.Models.Election> GetCurrentElectionsAsync()
        => Elections.Where(x => x.Status > ElectionStatus.None && x.Status < ElectionStatus.Ended).AsAsyncEnumerable();

    public IAsyncEnumerable<Abstractions.Models.Election> GetCurrentElectionsWithVotesAsync()
        => Elections.Include(election => election.Votes).Where(x => x.Status > ElectionStatus.None && x.Status < ElectionStatus.Ended).AsAsyncEnumerable();

    public async Task<Abstractions.Models.Election> GetCurrentElectionAsync(Direction direction)
        => await Elections.SingleAsync(x => x.Status > ElectionStatus.None && x.Status < ElectionStatus.Ended && x.Direction == direction).ConfigureAwait(false);

    public async IAsyncEnumerable<(short Id, Direction Direction, ElectionStatus Status)> GetCurrentElectionStatusAsync()
    {
        await foreach (var election in Elections
                           .Where(x => x.Status > ElectionStatus.None && x.Status < ElectionStatus.Ended)
                           .Select(x => new {x.ElectionId, x.Direction, x.Status})
                           .AsAsyncEnumerable().ConfigureAwait(false))
            yield return (election.ElectionId, election.Direction, election.Status);
    }

    public async ValueTask<(short Id, Direction Direction, ElectionStatus Status)> GetCurrentElectionStatusAsync(Direction direction)
    {
        var result = await Elections
            .Select(x => new {x.ElectionId, x.Direction, x.Status})
            .FirstAsync(x => x.Status > ElectionStatus.None && x.Status < ElectionStatus.Ended && x.Direction == direction)
            .ConfigureAwait(false);
        return (result.ElectionId, result.Direction, result.Status);
    }
}