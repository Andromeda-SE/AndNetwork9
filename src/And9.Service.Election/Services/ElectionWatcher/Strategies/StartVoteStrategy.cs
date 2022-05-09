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
    private readonly ReadAllMembersSender _allMembersSender;
    private readonly ElectionDataContext _electionDataContext;

    public StartVoteStrategy(ElectionDataContext electionDataContext, ReadAllMembersSender allMembersSender)
    {
        _electionDataContext = electionDataContext;
        _allMembersSender = allMembersSender;
    }

    public async Task UpdateElections()
    {
        ValueTask<Member[]> membersTask = _allMembersSender.CallAsync(0, CancellationToken.None).Where(x => x.Rank is > Rank.None and <= Rank.Advisor).ToArrayAsync();
        List<ValueTask<EntityEntry<ElectionVote>>> addTasks = new(128);

        await foreach (Abstractions.Models.Election election in _electionDataContext.GetCurrentElectionsWithVotesAsync().ConfigureAwait(false))
        {
            if (election.Status != ElectionStatus.Registration) throw new();
            election.AgainstAllVotes = 0;
            election.Status = ElectionStatus.Voting;
            addTasks.AddRange((await membersTask.ConfigureAwait(false))
                .ExceptBy(election.Votes.Where(x => x.Voted == null).Select(x => x.MemberId), x => x.Id)
                .Select(member => _electionDataContext.ElectionVotes.AddAsync(new()
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
        await _electionDataContext.SaveChangesAsync().ConfigureAwait(false);
    }
}