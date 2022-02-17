using And9.Integration.Discord.Database;
using And9.Integration.Discord.Database.Models;
using And9.Integration.Discord.Extensions;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using IConnection = RabbitMQ.Client.IConnection;

namespace And9.Integration.Discord.Listeners;

public class SyncRolesListener : BaseRabbitListenerWithoutResponse<object>
{
    private readonly DiscordDataContext _discordDataContext;
    private readonly DiscordBot _bot;

    public SyncRolesListener(IConnection connection,
        ILogger<BaseRabbitListenerWithoutResponse<object>> logger,
        DiscordDataContext discordDataContext,
        DiscordBot bot)
        : base(connection, SyncRolesSender.QUEUE_NAME, logger)
    {
        _discordDataContext = discordDataContext;
        _bot = bot;
    }

    public override async Task Run(object _)
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