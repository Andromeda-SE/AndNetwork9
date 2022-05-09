using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Abstractions.Interfaces;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace And9.Integration.Discord.ConsumerStrategies;

[QueueName(CreateChannelSender.QUEUE_NAME)]
public class CreateChannelConsumerStrategy : IBrokerConsumerWithResponseStrategy<IChannel, IChannel>
{
    private readonly DiscordBot _discordBot;
    private readonly DiscordDataContext _discordDataContext;
    private readonly RegisterChannelSender _registerChannelSender;
    private readonly SyncChannelsSender _syncChannelsSender;

    public CreateChannelConsumerStrategy(
        DiscordBot discordBot,
        DiscordDataContext discordDataContext,
        RegisterChannelSender registerChannelSender,
        SyncChannelsSender syncChannelsSender)
    {
        _discordBot = discordBot;
        _discordDataContext = discordDataContext;
        _registerChannelSender = registerChannelSender;
        _syncChannelsSender = syncChannelsSender;
    }

    public async ValueTask<IChannel> ExecuteAsync(IChannel? channel)
    {
        if (channel is null) throw new ArgumentNullException(nameof(channel));
        SocketGuild? guild = _discordBot.GetGuild(_discordBot.GuildId);
        ulong? category = channel.CategoryId.HasValue
            ? (await _discordDataContext.ChannelCategories.FindAsync(channel.CategoryId.Value).ConfigureAwait(false))?.DiscordId
            : null;
        global::Discord.IChannel discordChannel = channel.Type switch
        {
            DiscordChannelType.Text =>
                await guild.CreateTextChannelAsync(channel.Name,
                    properties => { properties.CategoryId = category; }).ConfigureAwait(false),
            DiscordChannelType.Voice =>
                await guild.CreateVoiceChannelAsync(channel.Name,
                    properties => { properties.CategoryId = category; }).ConfigureAwait(false),
            DiscordChannelType.Announcement => throw new NotSupportedException(),
            DiscordChannelType.Stage =>
                await guild.CreateStageChannelAsync(channel.Name,
                    properties => { properties.CategoryId = category; }).ConfigureAwait(false),
            DiscordChannelType.Thread => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException(),
        };
        await _registerChannelSender.CallAsync(channel).ConfigureAwait(false);
        await _syncChannelsSender.CallAsync(0).ConfigureAwait(false);
        return await _discordDataContext.Channels.FirstAsync(x => x.DiscordId == discordChannel.Id).ConfigureAwait(false);
    }
}