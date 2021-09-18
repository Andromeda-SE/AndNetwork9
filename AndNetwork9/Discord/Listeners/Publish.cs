using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Discord.Enums;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Discord.Listeners
{
    public class Publish : BaseRabbitListenerWithoutResponse<string>
    {
        private readonly DiscordBot _bot;
        private readonly IServiceScopeFactory _scopeFactory;

        public Publish(IConnection connection, DiscordBot bot, IServiceScopeFactory scopeFactory, ILogger<Publish> logger) : base(connection,
            PublishSender.QUEUE_NAME, logger)
        {
            _bot = bot;
            _scopeFactory = scopeFactory;
        }

        public override async Task Run(string arg)
        {
            AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
            ClanDataContext? data = (ClanDataContext?)scope.ServiceProvider.GetService(typeof(ClanDataContext));
            if (data is null) throw new ApplicationException();
            ulong channelId = data.DiscordChannels.Single(x => x.ChannelFlags.HasFlag(ChannelFlags.Advertisement))
                .DiscordId;
            await _bot.GetGuild(_bot.GuildId).GetTextChannel(channelId).SendMessageAsync(arg).ConfigureAwait(false);
        }
    }
}