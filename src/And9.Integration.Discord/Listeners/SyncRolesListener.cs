using System.Runtime.CompilerServices;
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
    private readonly DiscordBot _bot;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SyncRolesListener(IConnection connection,
        ILogger<BaseRabbitListenerWithoutResponse<object>> logger,
        DiscordBot bot,
        IServiceScopeFactory serviceScopeFactory)
        : base(connection, SyncRolesSender.QUEUE_NAME, logger)
    {
        _bot = bot;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task Run(object _)
    {
        SocketGuild? guild = _bot.GetGuild(_bot.GuildId);
        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable configuredAsyncDisposable = scope.ConfigureAwait(false);
        DiscordDataContext discordDataContext = scope.ServiceProvider.GetRequiredService<DiscordDataContext>();

        foreach (SocketRole? discordRole in guild.Roles.ExceptBy(discordDataContext.Roles.Select(x => x.DiscordId), role => role.Id))
        {
            if (discordRole.IsEveryone || discordRole.Tags.IsPremiumSubscriberRole) continue;
            await discordRole.DeleteAsync().ConfigureAwait(false);
        }

        await foreach (Role role in discordDataContext.Roles.ToAsyncEnumerable().OrderBy(x => x).ConfigureAwait(false))
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
                await discordDataContext.SaveChangesAsync().ConfigureAwait(false);
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