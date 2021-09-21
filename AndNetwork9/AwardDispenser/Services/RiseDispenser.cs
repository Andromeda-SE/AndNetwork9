using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.AwardDispenser.Services
{
    public class RiseDispenser : ITimerService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly PublishSender _publishSender;
        private readonly ILogger<RiseDispenser> _logger;

        public RiseDispenser(IServiceScopeFactory scopeFactory, IConfiguration configuration, PublishSender publishSender, ILogger<RiseDispenser> logger)
        {
            _scopeFactory = scopeFactory;
            _publishSender = publishSender;
            _logger = logger;
            Interval = TimeSpan.Parse(configuration["RISE_DISPENSER_INTERVAL"]);
        }

        CancellationTokenSource? ITimerService.CancellationTokenSource { get; set; }
        protected TimeSpan Interval { get; init; }
        TimeSpan ITimerService.Interval => Interval;

        PeriodicTimer? ITimerService.Timer { get; set; }

        public async Task Process()
        {
            _logger.LogInformation("Triggering " + nameof(RiseDispenser));
            AsyncServiceScope serviceScope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = serviceScope.ConfigureAwait(false);
            ClanDataContext data =
                (serviceScope.ServiceProvider.GetService(typeof(ClanDataContext)) as ClanDataContext)!;
            if (data is null) throw new();
            Member[] members = data.Members.Where(x => x.Rank > Rank.None && x.Rank < Rank.Advisor).ToArray();
            Dictionary<Member, (Rank oldRank, Rank newRank)> changes = new (); 
            foreach (Member member in members)
            {
                Rank actualRank = member.Awards.GetRank();
                if (member.Rank == actualRank) continue;
                changes.Add(member, (member.Rank, actualRank));
                member.Rank = actualRank;
            }
            if (!changes.Any()) return;
            await data.SaveChangesAsync();
            StringBuilder text = new(256);
            foreach ((Member member, (Rank oldRank, Rank newRank)) in changes.OrderByDescending(x => x.Key))
            {
                text.AppendLine(
                    oldRank > newRank
                        ? $"📉: Игрок {member.GetDiscordMention()} разжалован в ранг «{newRank.GetRankName()}»"
                        : $"📈: Игроку {member.GetDiscordMention()} присвоен ранг «{newRank.GetRankName()}»");
            }
            if (text.Length > 0) await _publishSender.CallAsync(text.ToString());
            _logger.LogInformation("Triggered " + nameof(RiseDispenser) + Environment.NewLine + $"Interval = {Interval.TotalSeconds}s");
        }
    }
}