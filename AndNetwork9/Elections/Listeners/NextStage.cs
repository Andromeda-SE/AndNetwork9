using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Elections;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Backend.Senders.Elections;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Elections.Listeners
{
    public class NextStage : BaseRabbitListenerWithoutResponse<ElectionStage>
    {
        private readonly RewriteElectionsChannelSender _rewriteElectionsChannelSender;
        private readonly IServiceScopeFactory _scopeFactory;

        public NextStage(IConnection connection, IServiceScopeFactory scopeFactory,
            RewriteElectionsChannelSender rewriteElectionsChannelSender, ILogger<NextStage> logger) : base(connection, NextStageSender.QUEUE_NAME, logger)
        {
            _scopeFactory = scopeFactory;
            _rewriteElectionsChannelSender = rewriteElectionsChannelSender;
        }

        public override async Task Run(ElectionStage _)
        {
            AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable __ = scope.ConfigureAwait(false);
            ClanDataContext data = (ClanDataContext)scope.ServiceProvider.GetService(typeof(ClanDataContext))!;
            if (data is null) throw new ApplicationException();

            Election? election =
                await data.Elections.FirstOrDefaultAsync(x =>
                    // ReSharper disable once MergeIntoPattern
                    x.Stage > ElectionStage.None && x.Stage < ElectionStage.Ended).ConfigureAwait(false);

            if (election is null)
            {
                EntityEntry<Election> result = await data.Elections.AddAsync(GetNewElections()).ConfigureAwait(false);
                election = result.Entity;
            }
            else
            {
                switch (election.Stage)
                {
                    case ElectionStage.Registration:
                        StartVoting(election, data);
                        break;
                    case ElectionStage.Voting:
                        election.Stage = ElectionStage.Announcement;
                        break;
                    case ElectionStage.Announcement:
                        EndElection(election, data);
                        await data.Elections.AddAsync(GetNewElections()).ConfigureAwait(false);
                        break;
                }
            }

            await data.SaveChangesAsync().ConfigureAwait(false);
            await _rewriteElectionsChannelSender.CallAsync(election).ConfigureAwait(false);
        }

        private static Election GetNewElections()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            return new()
            {
                Stage = ElectionStage.Registration,
                RegistrationDate = today,
                VoteDate = today.AddDays(ClanRulesExtensions.ADVISOR_TERM_DAYS
                                         - ClanRulesExtensions.VOTING_DAYS
                                         - ClanRulesExtensions.ANNOUNCEMENT_DAYS),
                AnnouncementDate = today.AddDays(ClanRulesExtensions.ADVISOR_TERM_DAYS
                                                 - ClanRulesExtensions.ANNOUNCEMENT_DAYS),
                StartDate = today.AddDays(ClanRulesExtensions.ADVISOR_TERM_DAYS),
                EndDate = today.AddDays(ClanRulesExtensions.ADVISOR_TERM_DAYS * 2),
                Votings = Enum.GetValues<Direction>().Where(x => x > Direction.None).Select(x => new ElectionVoting
                {
                    Members = new List<ElectionsMember>(),
                    Direction = x,
                    AgainstAll = 0,
                }).ToList(),
            };
        }

        private static void StartVoting(Election election, ClanDataContext data)
        {
            foreach (ElectionVoting voting in election.Votings)
            {
                Member[] candidates = voting.Members.Select(x => x.Member).ToArray();
                foreach (Member member in data.Members.AsEnumerable()
                    .Where(x => x.Rank > Rank.None).ToArray())
                    voting.Members.Add(new()
                    {
                        Direction = voting.Direction,
                        ElectionId = voting.ElectionId,
                        MemberId = member.Id,
                        Voted = false,
                        Votes = null,
                        Voting = voting,
                        Member = member,
                    });
            }

            election.Stage = ElectionStage.Voting;
        }

        private static void EndElection(Election election, ClanDataContext data)
        {
            election.Stage = ElectionStage.Ended;

            foreach (Member oldAdvisor in data.Members.Where(x => x.Rank == Rank.Advisor).ToArray())
                oldAdvisor.Rank = oldAdvisor.Awards.GetRank();

            foreach (ElectionVoting voting in election.Votings.ToArray())
            {
                Member? winner = voting.GetWinner();
                if (winner is not null) winner.Rank = Rank.Advisor;
            }
        }
    }
}