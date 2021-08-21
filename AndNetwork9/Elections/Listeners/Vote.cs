using System;
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

namespace AndNetwork9.Elections.Listeners
{
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
            AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
            ClanDataContext data = (ClanDataContext)scope.ServiceProvider.GetService(typeof(ClanDataContext))!;
            if (data is null) throw new ApplicationException();


            Election? election;
            try
            {
                election = await data.Elections.SingleAsync(x => x.Stage == ElectionStage.Registration).ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
                throw new FailedCallException(HttpStatusCode.Gone);
            }

            ElectionVoting voting = election.Votings.Single(x => x.Direction == request.Direction);
            long votesTrueCount = voting.Members.Count(x => x.Votes is not null);
            long votesCount = request.Votes.Sum(x => x.Votes);
            if (votesTrueCount != votesCount) throw new FailedCallException(HttpStatusCode.BadRequest);
            if (request.Votes
                .Where(x => x.MemberId.HasValue)
                .Select(x => x.MemberId!.Value)
                .Except(
                    voting.Members.Where(x => x.Votes is not null).Select(x => x.MemberId))
                .Any())
                throw new FailedCallException(HttpStatusCode.BadRequest);
            if (request.Votes.Count(x => x.MemberId is null) > 1)
                throw new FailedCallException(HttpStatusCode.BadRequest);
            ElectionsMember? electionMember = voting.Members.FirstOrDefault(x =>
                x.MemberId == request.MemberId && x.VoterKey == request.Key && !x.Voted);
            if (electionMember is null) throw new FailedCallException(HttpStatusCode.NotFound);

            foreach (VoteArgNode requestVote in request.Votes.Where(x => x.MemberId is not null))
            {
                ElectionsMember candidate = voting.Members.Single(x => x.MemberId == requestVote.MemberId);
                candidate.Votes += (int)requestVote.Votes;
            }

            if (request.Votes.Any(x => x.MemberId is null))
                voting.AgainstAll += (int)request.Votes.Single(x => x.MemberId is null).Votes;

            await data.SaveChangesAsync().ConfigureAwait(false);
            await _rewriteElectionsChannelSender.CallAsync(election).ConfigureAwait(false);
        }
    }
}