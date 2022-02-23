using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Database;
using And9.Service.Election.Database.Models;
using And9.Service.Election.Senders;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace And9.Service.Election.Listeners;

public class VoteListener : BaseRabbitListenerWithResponse<(int MemberId, IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>> Votes), bool>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    protected VoteListener(IConnection connection,  ILogger<VoteListener> logger, IServiceScopeFactory serviceScopeFactory) : base(connection, VoteSender.QUEUE_NAME, logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task<bool> GetResponseAsync((int MemberId, IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>> Votes) request)
    {
        (int memberId, IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>>? votes) = request;
        if (votes.Count != Enum.GetValues<Direction>().Count(x => x > Direction.None)) return false;
        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        ElectionDataContext context = scope.ServiceProvider.GetRequiredService<ElectionDataContext>();

        (short id, ElectionStatus status) = await context.GetCurrentElectionStatusAsync().ConfigureAwait(false);
        if (status is not ElectionStatus.Voting) return false;

        foreach (var item in context.ElectionVotes
                     .Where(x => x.ElectionId == memberId && x.ElectionId == id)
                     .Join(votes, vote => vote.Direction, pair => pair.Key, (vote, pair) => new {ElectionVote = vote, Votes = pair.Value}))
        {
            if (item.ElectionVote.Voted is not false) return false;
            item.ElectionVote.Voted = true;
            int totalVotesCount = await context.ElectionVotes.CountAsync(x => x.ElectionId == id && x.Direction == item.ElectionVote.Direction).ConfigureAwait(false);
            int actualVotesCount = 0;
            foreach ((int? candidateId, int votesCount) in item.Votes)
            {
                actualVotesCount += votesCount;
                if (candidateId is null)
                {
                    Abstractions.Models.Election election = await context.Elections.SingleAsync(x => x.ElectionId == id && x.Direction == item.ElectionVote.Direction).ConfigureAwait(false);
                    election.AgainstAllVotes += votesCount;
                }
                else
                {
                    ElectionVote candidate = await context.ElectionVotes.SingleAsync(x =>
                        x.ElectionId == id
                        && x.Direction == item.ElectionVote.Direction
                        && x.MemberId == candidateId
                        && x.Voted == null).ConfigureAwait(false);
                    candidate.Votes += votesCount;
                }
            }
            if (totalVotesCount != actualVotesCount) return false;
        }

        await context.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }
}