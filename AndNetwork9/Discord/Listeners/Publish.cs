using System;
using System.Linq;
using System.Net;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Discord.Enums;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Discord;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace AndNetwork9.Discord.Listeners
{
    public class Publish : BaseRabbitListenerWithoutResponse<string>
    {
        private readonly DiscordBot _bot;
        private readonly IServiceScopeFactory _scopeFactory;

        public Publish(IConnection connection, DiscordBot bot, IServiceScopeFactory scopeFactory) : base(connection,
            PublishSender.QUEUE_NAME)
        {
            _bot = bot;
            _scopeFactory = scopeFactory;
        }

        public override async void Run(string arg)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            ClanDataContext? data = (ClanDataContext?)scope.ServiceProvider.GetService(typeof(ClanDataContext));
            if (data is null) throw new ApplicationException();
            try
            {
                ulong channelId = data.DiscordChannels.Single(x => x.ChannelFlags.HasFlag(ChannelFlags.Advertisement))
                    .DiscordId;
                await _bot.GetGuild(_bot.GuildId).GetTextChannel(channelId).SendMessageAsync(arg);
            }
            catch (Exception e)
            {
                throw new FailedCallException(HttpStatusCode.Locked)
                {
                    Description = e.Message,
                };
            }
        }
    }
}