using System.Runtime.CompilerServices;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Abstractions.Models;
using And9.Service.Election.Database;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace And9.Service.Election.Services.ElectionWatcher.Strategies;

public class StartVoteStrategy : IElectionWatcherStrategy
{
    private readonly MemberCrudSender _memberCrudSender;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public StartVoteStrategy(MemberCrudSender memberCrudSender, IServiceScopeFactory serviceScopeFactory)
    {
        _memberCrudSender = memberCrudSender;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task UpdateElections()
    {
        ValueTask<Member[]> membersTask = _memberCrudSender.ReadAll(CancellationToken.None).Where(x => x.Rank is > Rank.None and <= Rank.Advisor).ToArrayAsync();
        List<ValueTask<EntityEntry<ElectionVote>>> addTasks = new(128);
        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable configuredAsyncDisposable = scope.ConfigureAwait(false);
        ElectionDataContext electionDataContext = scope.ServiceProvider.GetRequiredService<ElectionDataContext>();
        await foreach (Abstractions.Models.Election election in electionDataContext.GetCurrentElectionsWithVotesAsync().ConfigureAwait(false))
        {
            if (election.Status != ElectionStatus.Registration) throw new();
            election.AgainstAllVotes = 0;
            election.Status = ElectionStatus.Voting;
            addTasks.AddRange((await membersTask.ConfigureAwait(false))
                .ExceptBy(election.Votes.Where(x => x.Voted == null).Select(x => x.MemberId), x => x.Id)
                .Select(member => electionDataContext.ElectionVotes.AddAsync(new()
                {
                    ElectionId = election.ElectionId,
                    Direction = election.Direction,
                    MemberId = member.Id,
                    Votes = 0,
                    Voted = false,
                    Election = election,
                })));
        }

        await Task.WhenAll(addTasks.Select(x => x.AsTask())).ConfigureAwait(false);
        await electionDataContext.SaveChangesAsync().ConfigureAwait(false);
    }
}