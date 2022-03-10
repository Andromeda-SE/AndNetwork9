using System.Data.Common;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Award.Database;

public class AwardDataContext : DbContext
{
    protected const string POINTS_EXPRESSION = "COALESCE(ROUND(SUM(\"Type\" * pow(2, (\"Date\" - now()::DATE) / 365.25)), 10), 0)";
    public AwardDataContext(DbContextOptions<AwardDataContext> options) : base(options) { }
    public DbSet<Abstractions.Models.Award> Awards { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Award");

        modelBuilder.Entity<Abstractions.Models.Award>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.MemberId);
            entity.Property(x => x.MemberId);
            entity.Property(x => x.Type);
            entity.Property(x => x.Description);
            entity.Property(x => x.Date);
            entity.Property(x => x.GaveById);
            entity.HasIndex(x => x.AutomationTag);
            entity.Property(x => x.AutomationTag);

            /*entity.Property(x => x.Points)
                .HasComputedColumnSql(
                    $"round(\"{nameof(Abstractions.Models.Award.Type)}\" * pow(2, (\"{nameof(Abstractions.Models.Award.Date)}\" - now()::date) / 365.25), 10)",
                    false);*/
            entity.Property(x => x.ConcurrencyToken).IsRowVersion().HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.LastChanged).IsRowVersion().HasDefaultValueSql("now()");
        });
    }

    public virtual async Task<double> GetPointsAsync(int memberId)
    {
        await using DbCommand command = Database.GetDbConnection().CreateCommand();
        command.CommandText = @$"
SELECT {POINTS_EXPRESSION}
FROM ""Award"".""Awards""
WHERE ""MemberId"" = {memberId:D}";
        await Database.OpenConnectionAsync().ConfigureAwait(false);
        await using DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        await reader.ReadAsync().ConfigureAwait(false);
        return reader.GetDouble(0);
    }

    public virtual async IAsyncEnumerable<(int MemberId, double Points)> GetPointsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using DbCommand command = Database.GetDbConnection().CreateCommand();
        command.CommandText = @$"
SELECT ""Award"".""Awards"".""MemberId"",
       {POINTS_EXPRESSION}
FROM ""Award"".""Awards""
GROUP BY ""Award"".""Awards"".""MemberId""";
        await Database.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false)) yield return (reader.GetInt32(0), reader.GetDouble(1));
    }
}