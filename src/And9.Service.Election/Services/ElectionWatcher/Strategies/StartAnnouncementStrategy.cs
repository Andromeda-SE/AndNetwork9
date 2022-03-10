using System.Runtime.CompilerServices;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Database;

namespace And9.Service.Election.Services.ElectionWatcher.Strategies;

public class StartAnnouncementStrategy : IElectionWatcherStrategy
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public StartAnnouncementStrategy(IServiceScopeFactory serviceScopeFactory) => _serviceScopeFactory = serviceScopeFactory;

    public async Task UpdateElections()
    {
        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable configuredAsyncDisposable = scope.ConfigureAwait(false);
        ElectionDataContext electionDataContext = scope.ServiceProvider.GetRequiredService<ElectionDataContext>();
        await foreach (Abstractions.Models.Election election in electionDataContext.GetCurrentElectionsAsync().ConfigureAwait(false))
        {
            if (election.Status != ElectionStatus.Voting) throw new();
            election.Status = ElectionStatus.Announcement;
        }

        await electionDataContext.SaveChangesAsync().ConfigureAwait(false);
    }
}