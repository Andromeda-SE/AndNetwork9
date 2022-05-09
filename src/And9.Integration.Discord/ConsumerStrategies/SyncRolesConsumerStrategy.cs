using And9.Integration.Discord.Database;
using And9.Integration.Discord.Database.Models;
using And9.Integration.Discord.Extensions;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace And9.Integration.Discord.ConsumerStrategies;

[QueueName(SyncRolesSender.QUEUE_NAME)]
public class SyncRolesConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<object>
{
    private readonly DiscordBot _bot;
    private readonly DiscordDataContext _discordDataContext;

    public SyncRolesConsumerStrategy(DiscordBot bot, DiscordDataContext discordDataContext)
    {
        _bot = bot;
        _discordDataContext = discordDataContext;
    }

    public async ValueTask ExecuteAsync(object _)
    {
        SocketGuild? guild = _bot.GetGuild(_bot.GuildId);

        foreach (SocketRole? discordRole in guild.Roles.ExceptBy(_discordDataContext.Roles.Select(x => x.DiscordId), role => role.Id))
        {
            if (discordRole.IsEveryone || discordRole.Tags.IsPremiumSubscriberRole) continue;
            await discordRole.DeleteAsync().ConfigureAwait(false);
        }

        await foreach (Role role in _discordDataContext.Roles.ToAsyncEnumerable().OrderBy(x => x).ConfigureAwait(false))
        {
            SocketRole? discordRole = guild.GetRole(role.DiscordId);
            if (discordRole is null)
            {
                RestRole? newRole = await guild.CreateRoleAsync(
                    role.Name,
                    role.GlobalPermissions.ToGuildPermissions(),
                    role.Color.HasValue ? new(role.Color.Value.R, role.Color.Value.G, role.Color.Value.B) : null,
                    role.IsHoisted,
                    role.IsMentionable,
                    RequestOptions.Default).ConfigureAwait(false);
                role.DiscordId = newRole.Id;
                await _discordDataContext.SaveChangesAsync().ConfigureAwait(false);
                continue;
            }

            await discordRole.ModifyAsync(properties =>
            {
                properties.Name = role.Name;
                properties.Color = role.Color.HasValue
                    ? new(new(role.Color.Value.R, role.Color.Value.G, role.Color.Value.B))
                    : Optional<Color>.Unspecified;
                properties.Hoist = role.IsHoisted;
                properties.Mentionable = role.IsMentionable;
                properties.Permissions = role.GlobalPermissions.ToGuildPermissions();
            }).ConfigureAwait(false);
        }
    }
}