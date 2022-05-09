using System.Runtime.CompilerServices;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Database;
using And9.Service.Election.Services.ElectionWatcher.Strategies;

namespace And9.Service.Election.Services.ElectionWatcher;

public sealed class ElectionWatcher : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ElectionWatcher(IServiceScopeFactory serviceScopeFactory) => _serviceScopeFactory = serviceScopeFactory;

    public CancellationTokenSource? CancellationTokenSource { get; private set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable configuredAsyncDisposable = scope.ConfigureAwait(false);
        ElectionDataContext electionDataContext = scope.ServiceProvider.GetRequiredService<ElectionDataContext>();
        int electionsCount = await electionDataContext.GetCurrentElectionsAsync().CountAsync(cancellationToken).ConfigureAwait(false);
        if (electionsCount == 0)
        {
            await electionDataContext.Elections.AddRangeAsync(Enum.GetValues<Direction>().Where(x => x > Direction.None).Select(x => new Abstractions.Models.Election
                {
                    AdvisorsStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(90)),
                    AgainstAllVotes = 0,
                    Status = ElectionStatus.Registration,
                    Direction = x,
                    ConcurrencyToken = Guid.NewGuid(),
                    LastChanged = DateTime.UtcNow,
                }),
                cancellationToken).ConfigureAwait(false);
            await electionDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
        CancellationTokenSource = new();
        _ = Task.Run(() => Run(CancellationTokenSource.Token), cancellationToken);
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
        return Task.CompletedTask;
    }

    private async Task Run(CancellationToken token)
    {
        bool needWait;
        do
        {
            AsyncServiceScope localScope = _serviceScopeFactory.CreateAsyncScope();
            ElectionDataContext dataContext = localScope.ServiceProvider.GetRequiredService<ElectionDataContext>();
            Abstractions.Models.Election election = await dataContext.GetCurrentElectionAsync(Direction.Infrastructure).ConfigureAwait(false);
            int daysStage = election.Status switch
            {
                ElectionStatus.Registration => 15,
                ElectionStatus.Voting => 5,
                ElectionStatus.Announcement => 0,
                _ => throw new ArgumentOutOfRangeException(),
            };
            DateOnly date = election.AdvisorsStartDate.AddDays(-daysStage);
            needWait = date > DateOnly.FromDateTime(DateTime.Today);
            await Task.Delay(DateTime.Today.AddDays(1) - DateTime.Now, token).ConfigureAwait(false);
        } while (needWait);

        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        ElectionDataContext electionDataContext = scope.ServiceProvider.GetRequiredService<ElectionDataContext>();
        if (token.IsCancellationRequested) return;
        Abstractions.Models.Election[] elections = await electionDataContext.GetCurrentElectionsAsync().ToArrayAsync(token).ConfigureAwait(false);
        IElectionWatcherStrategy electionWatcherStrategy = elections.First().Status switch
        {
            ElectionStatus.Registration => scope.ServiceProvider.GetRequiredService<StartVoteStrategy>(),
            ElectionStatus.Voting => scope.ServiceProvider.GetRequiredService<StartAnnouncementStrategy>(),
            ElectionStatus.Announcement => scope.ServiceProvider.GetRequiredService<NewElectionStrategy>(),
            _ => throw new ArgumentOutOfRangeException(),
        };
        await electionWatcherStrategy.UpdateElections().ConfigureAwait(false);
    }
}