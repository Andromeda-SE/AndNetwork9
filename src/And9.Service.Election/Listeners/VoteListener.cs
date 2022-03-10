using And9.Gateway.Clan.Senders;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Abstractions.Models;
using And9.Service.Election.Database;
using And9.Service.Election.Senders;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace And9.Service.Election.Listeners;

public class VoteListener : BaseRabbitListenerWithResponse<(int MemberId, IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>> Votes), bool>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public VoteListener(IConnection connection, ILogger<VoteListener> logger, IServiceScopeFactory serviceScopeFactory) : base(connection, VoteSender.QUEUE_NAME, logger) => _serviceScopeFactory = serviceScopeFactory;

    protected override async Task<bool> GetResponseAsync((int MemberId, IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>> Votes) request)
    {
        (int memberId, IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>>? allVotes) = request;
        if (allVotes.Count != Enum.GetValues<Direction>().Count(x => x > Direction.None)) return false;
        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        ElectionDataContext context = scope.ServiceProvider.GetRequiredService<ElectionDataContext>();

        List<int> electionsNeedUpdate = new(8);
        await foreach ((Abstractions.Models.Election election, IReadOnlyDictionary<int?, int> votes) in
                       context.GetCurrentElectionsWithVotesAsync()
                           .Join(allVotes.ToAsyncEnumerable(), x => x.Direction, x => x.Key, (election, pair) => (election, pair.Value))
                           .ConfigureAwait(false))
        {
            if (election.Status != ElectionStatus.Voting) return false;
            ElectionVote electionVote = await context.ElectionVotes
                .FirstAsync(x => x.ElectionId == election.ElectionId && x.Direction == election.Direction && x.MemberId == memberId)
                .ConfigureAwait(false);

            if (electionVote.Voted is not false) return false;
            electionVote.Voted = true;
            int totalVotesCount = await context.ElectionVotes.CountAsync(x => x.ElectionId == election.ElectionId && x.Direction == election.Direction && x.Voted == null).ConfigureAwait(false);
            int actualVotesCount = 0;
            foreach ((int? candidateId, int votesCount) in allVotes[election.Direction])
            {
                actualVotesCount += votesCount;
                if (candidateId is null)
                {
                    election.AgainstAllVotes += votesCount;
                }
                else
                {
                    ElectionVote candidate = await context.ElectionVotes.SingleAsync(x =>
                        x.ElectionId == election.ElectionId
                        && x.Direction == election.Direction
                        && x.MemberId == candidateId
                        && x.Voted == null).ConfigureAwait(false);
                    candidate.Votes += votesCount;
                }
            }

            if (totalVotesCount != actualVotesCount) return false;
            electionsNeedUpdate.Add(election.ElectionId);
        }

        await context.SaveChangesAsync().ConfigureAwait(false);
        RaiseElectionUpdateSender sender = scope.ServiceProvider.GetRequiredService<RaiseElectionUpdateSender>();
        await Task.WhenAll(electionsNeedUpdate.Select(x => sender.CallAsync(x))).ConfigureAwait(false);
        return true;
    }
}