using And9.Integration.Discord.Abstractions.Interfaces;
using And9.Integration.Discord.Abstractions.Models;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;

namespace And9.Integration.Discord.ConsumerStrategies;

[QueueName(UpdateChannelSender.QUEUE_NAME)]
public class UpdateChannelConsumerStrategy : IBrokerConsumerWithResponseStrategy<IChannel, IChannel>
{
    private readonly DiscordDataContext _discordDataContext;
    private readonly SyncChannelsSender _syncChannelsSender;

    public UpdateChannelConsumerStrategy(
        DiscordDataContext discordDataContext,
        SyncChannelsSender syncChannelsSender)
    {
        _discordDataContext = discordDataContext;
        _syncChannelsSender = syncChannelsSender;
    }

    public async ValueTask<IChannel> ExecuteAsync(IChannel? channel)
    {
        if (channel is null) throw new ArgumentNullException(nameof(channel));
        Channel? oldChannel = await _discordDataContext.Channels.FindAsync(channel.DiscordId).ConfigureAwait(false);
        if (oldChannel is null) throw new ArgumentException();

        Channel newChannel = oldChannel with
        {
            Name = channel.Name,
            DiscordChannelFlags = channel.DiscordChannelFlags,
            CategoryId = channel.CategoryId,
            ChannelPosition = channel.ChannelPosition,
            Direction = channel.Direction,
            SquadNumber = channel.SquadNumber,
            SquadPartNumber = channel.SquadPartNumber,
            EveryonePermissions = channel.AdvisorPermissions,
            MemberPermissions = channel.AdvisorPermissions,
            SquadPartPermissions = channel.AdvisorPermissions,
            SquadPartCommanderPermissions = channel.AdvisorPermissions,
            SquadPermissions = channel.AdvisorPermissions,
            SquadLieutenantsPermissions = channel.AdvisorPermissions,
            SquadCaptainPermissions = channel.AdvisorPermissions,
            AdvisorPermissions = channel.AdvisorPermissions,
        };
        _discordDataContext.Channels.Update(newChannel);
        await _discordDataContext.SaveChangesAsync().ConfigureAwait(false);
        await _syncChannelsSender.CallAsync(0).ConfigureAwait(false);
        return oldChannel;
    }
}