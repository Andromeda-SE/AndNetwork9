using And9.Gateway.Clan.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Abstractions.Models;
using And9.Service.Election.Database;
using And9.Service.Election.Senders;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Election.ConsumerStrategies;

[QueueName(VoteSender.QUEUE_NAME)]
public class VoteConsumerStrategy : IBrokerConsumerWithResponseStrategy<(int MemberId, IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>> Votes), bool>
{
    private readonly ElectionDataContext _electionDataContext;
    private readonly RaiseElectionUpdateSender _raiseElectionUpdateSender;

    public VoteConsumerStrategy(ElectionDataContext electionDataContext, RaiseElectionUpdateSender raiseElectionUpdateSender)
    {
        _electionDataContext = electionDataContext;
        _raiseElectionUpdateSender = raiseElectionUpdateSender;
    }

    public async ValueTask<bool> ExecuteAsync((int MemberId, IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>> Votes) request)
    {
        (int memberId, IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>>? allVotes) = request;
        if (allVotes.Count != Enum.GetValues<Direction>().Count(x => x > Direction.None)) return false;

        List<int> electionsNeedUpdate = new(8);
        await foreach ((Abstractions.Models.Election election, IReadOnlyDictionary<int?, int> votes) in
                       _electionDataContext.GetCurrentElectionsWithVotesAsync()
                           .Join(allVotes.ToAsyncEnumerable(), x => x.Direction, x => x.Key, (election, pair) => (election, pair.Value))
                           .ConfigureAwait(false))
        {
            if (election.Status != ElectionStatus.Voting) return false;
            ElectionVote electionVote = await _electionDataContext.ElectionVotes
                .FirstAsync(x => x.ElectionId == election.ElectionId && x.Direction == election.Direction && x.MemberId == memberId)
                .ConfigureAwait(false);

            if (electionVote.Voted is not false) return false;
            electionVote.Voted = true;
            int totalVotesCount = await _electionDataContext.ElectionVotes.CountAsync(x => x.ElectionId == election.ElectionId && x.Direction == election.Direction && x.Voted == null).ConfigureAwait(false);
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
                    ElectionVote candidate = await _electionDataContext.ElectionVotes.SingleAsync(x =>
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

        await _electionDataContext.SaveChangesAsync().ConfigureAwait(false);
        await Task.WhenAll(electionsNeedUpdate.Select(x => _raiseElectionUpdateSender.CallAsync(x).AsTask())).ConfigureAwait(false);
        return true;
    }
}