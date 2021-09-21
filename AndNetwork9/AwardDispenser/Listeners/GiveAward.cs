using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.AwardDispenser;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.AwardDispenser.Listeners
{
    public class GiveAward : BaseRabbitListenerWithoutResponse<Award>
    {
        private readonly List<Award> _awards = new(16);
        private readonly PublishSender _publishSender;
        private readonly IServiceScopeFactory _scopeFactory;
        private CancellationTokenSource _tokenSource = new();

        public GiveAward(IConnection connection, ILogger<BaseRabbitListenerWithoutResponse<Award>> logger,
            IServiceScopeFactory scopeFactory, PublishSender publishSender)
            : base(connection, GiveAwardSender.QUEUE_NAME, logger)
        {
            _scopeFactory = scopeFactory;
            _publishSender = publishSender;
        }

        public override async Task Run(Award request)
        {
            using IServiceScope serviceScope = _scopeFactory.CreateScope();
            ClanDataContext data =
                (serviceScope.ServiceProvider.GetService(typeof(ClanDataContext)) as ClanDataContext)!;
            if (data is null) throw new();

            Member? member = data.Members.FirstOrDefault(x => x.Id == request.MemberId);
            if (member is null) throw new ArgumentException(nameof(request));
            member.Awards.Add(request);
            await data.SaveChangesAsync().ConfigureAwait(false);

            _tokenSource.Cancel(false);
            _tokenSource.Dispose();
            _tokenSource = new();
            request.Member ??= member;
            lock (_awards)
            {
                _awards.Add(request);
            }

            RunPublishTask(_tokenSource.Token);
        }

        private void RunPublishTask(CancellationToken cancellationToken = default)
        {
            Task.Run(async () => await TriggerPublish(cancellationToken).ConfigureAwait(false), CancellationToken.None);
        }

        private async Task TriggerPublish(CancellationToken cancellationToken = default)
        {
            await Task.Delay(new TimeSpan(0, 0, 30), cancellationToken).ConfigureAwait(false);
            string text;
            lock (_awards)
            {
                if (!_awards.Any()) return;
                _awards.Sort();
                _awards.Reverse();
                if (cancellationToken.IsCancellationRequested) return;
                text = string.Join(Environment.NewLine,
                    _awards.Select(x =>
                        $"{x.Type.GetAwardSymbol()}: {x.Type.GetTypeName()} «{x.Description}» достается игроку {x.Member!.GetDiscordMention()}"));
                _awards.Clear();
            }

            if (text.Length > 0) await _publishSender.CallAsync(text).ConfigureAwait(false);
        }
    }
}