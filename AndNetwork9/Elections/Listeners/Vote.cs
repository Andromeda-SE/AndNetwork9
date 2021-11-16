using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Elections;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Backend.Senders.Elections;
using AndNetwork9.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Elections.Listeners;

public class Vote : BaseRabbitListenerWithoutResponse<VoteArg>
{
    private readonly RewriteElectionsChannelSender _rewriteElectionsChannelSender;
    private readonly IServiceScopeFactory _scopeFactory;

    public Vote(IConnection connection,
        IServiceScopeFactory scopeFactory,
        RewriteElectionsChannelSender rewriteElectionsChannelSender,
        ILogger<Vote> logger) : base(connection, VoteSender.QUEUE_NAME, logger)
    {
        _scopeFactory = scopeFactory;
        _rewriteElectionsChannelSender = rewriteElectionsChannelSender;
    }

    public override async Task Run(VoteArg request)
    {
        try
        {
            Logger.LogInformation("Start process voting…");
            Logger.LogInformation(
                $"MemberId: {request.MemberId}{Environment.NewLine}{string.Join(Environment.NewLine, request.Bulletin.Select(x => $"{x.Key} {string.Join(", ", x.Value.Select(y => $"{y.Key}-{y.Value}"))}"))}");
            AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
            ClanDataContext data = (ClanDataContext)scope.ServiceProvider.GetService(typeof(ClanDataContext))!;
            if (data is null) throw new ApplicationException();
            Election? election;
            try
            {
                election = await data.Elections.SingleAsync(x => x.Stage == ElectionStage.Voting)
                    .ConfigureAwait(false);
                Logger.LogInformation($"Election = {election.Id}");
            }
            catch (InvalidOperationException)
            {
                throw new FailedCallException(HttpStatusCode.Gone);
            }

            foreach ((Direction direction, IReadOnlyDictionary<int, uint> votes) in request.Bulletin)
            {
                Logger.LogInformation($"Process {election.Id}/{direction}…");
                long votesCount = votes.Values.Sum(x => x);
                ElectionVoting voting = election.Votings.Single(x => x.Direction == direction);
                long votesTrueCount = voting.Members.Count(x => x.Votes is not null);
                Logger.LogInformation($"{election.Id}/{direction}: {votesCount}/{votesTrueCount}");
                if (votesTrueCount != votesCount) throw new FailedCallException(HttpStatusCode.BadRequest);
                if (votes.Keys.Where(x => x != 0)
                    .Except(
                        voting.Members.Where(x => x.Votes is not null).Select(x => x.MemberId))
                    .Any()) throw new FailedCallException(HttpStatusCode.BadRequest);
                ElectionsMember? electionMember = voting.Members.FirstOrDefault(x =>
                    x.MemberId == request.MemberId && !x.Voted);
                if (electionMember is null) throw new FailedCallException(HttpStatusCode.NotFound);
                electionMember.Voted = true;
                if (votesCount == 0) continue;
                Dictionary<ElectionsMember, int>? candidatesVotes = voting.Members
                    .Join(votes, x => x.MemberId, x => x.Key, (member, pair) => (member, (int)pair.Value))
                    .ToDictionary(x => x.member, x => x.Item2);
                foreach ((ElectionsMember candidate, int vote) in candidatesVotes)
                {
                    if (vote == 0) continue;
                    candidate.Votes += vote;
                }

                if (votes.TryGetValue(0, out uint againstAllVotes)) voting.AgainstAll += (int)againstAllVotes;
            }

            await data.SaveChangesAsync().ConfigureAwait(false);
            await _rewriteElectionsChannelSender.CallAsync(election).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Logger.LogError(e, request.MemberId.ToString());
            throw;
        }
    }
}