using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AndNetwork9.AwardDispenser.Services.AwardDispenserJobs;
using AndNetwork9.AwardDispenser.Services.AwardDispenserJobs.Interfaces;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.AwardDispenser;
using AndNetwork9.Shared.Backend.Senders.Steam;
using AndNetwork9.Shared.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.AwardDispenser.Services
{
    public class AwardDispenser : ITimerService
    {
        private static readonly HashSet<IAwardDispenserJob> Jobs = new()
        {
            new InGameAwardDispenserJob(),
            new WithComradesAwardDispenserJob(),
        };

        private readonly PlayerActivitySender _playerActivitySender;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly GiveAwardSender _giveAwardSender;
        private readonly ILogger<AwardDispenser> _logger;

        public AwardDispenser(IConfiguration configuration, IServiceScopeFactory scopeFactory,
            PlayerActivitySender playerActivitySender, GiveAwardSender giveAwardSender, ILogger<AwardDispenser> logger)
        {
            _scopeFactory = scopeFactory;
            _playerActivitySender = playerActivitySender;
            _giveAwardSender = giveAwardSender;
            _logger = logger;
            Interval = TimeSpan.Parse(configuration["AWARD_DISPENSER_INTERVAL"]);
        }

        protected TimeSpan Interval { get; init; }

        CancellationTokenSource? ITimerService.CancellationTokenSource { get; set; }

        PeriodicTimer? ITimerService.Timer { get; set; }

        TimeSpan ITimerService.Interval => Interval;

        public async Task Process()
        {
            _logger.LogInformation("Triggering " + nameof(AwardDispenser));
            AsyncServiceScope serviceScope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = serviceScope.ConfigureAwait(false);
            ClanDataContext data =
                (serviceScope.ServiceProvider.GetService(typeof(ClanDataContext)) as ClanDataContext)!;
            if (data is null) throw new();
            Member[] members = data.Members.Where(x => x.Rank > Rank.None && x.Rank < Rank.Advisor).ToArray();
            List<PlayerActivityResultNode> nodes = new();
            for (int i = 0; i < members.Length; i += 100)
                nodes.AddRange(
                    await _playerActivitySender.CallAsync(members.Skip(i).Take(i + 100).Select(x => x.SteamId)
                        .ToArray()).ConfigureAwait(false)
                    ?? throw new());

            foreach (IAwardDispenserJob job in Jobs)
            {
                job.PlayerActivity = nodes;
                foreach (Member member in (await job.AvailableAsync(members).ConfigureAwait(false)).Where(x => x.Value)
                    .Select(x => x.Key))
                {
                    await _giveAwardSender.CallAsync(new()
                    {
                        AutomationTag = job.AutomationTag,
                        Member = member,
                        Description = job.Description,
                        Date = DateOnly.FromDateTime(DateTime.Today),
                        GaveBy = null,
                        GaveById = null,
                        MemberId = member.Id,
                        Type = job.AwardType,
                    }).ConfigureAwait(false);
                }
            }

            await data.SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);
            _logger.LogInformation("Triggered " + nameof(AwardDispenser) + Environment.NewLine + $"Interval = {Interval.TotalSeconds}s");
        }
    }
}