using System.Runtime.CompilerServices;
using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.Database.Models;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using Discord.WebSocket;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Listeners;

public class SyncUserListener : BaseRabbitListenerWithoutResponse<Member>
{
    private readonly DiscordBot _bot;
    private readonly IServiceScopeFactory _scopeFactory;

    public SyncUserListener(
        IConnection connection,
        ILogger<BaseRabbitListenerWithoutResponse<Member>> logger,
        DiscordBot bot,
        IServiceScopeFactory scopeFactory)
        : base(connection, SyncUserSender.QUEUE_NAME, logger)
    {
        _bot = bot;
        _scopeFactory = scopeFactory;
    }

    public override async Task Run(Member member)
    {
        if (!member.DiscordId.HasValue) throw new ArgumentException("DiscordId is null", nameof(member));
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        DiscordDataContext data = scope.ServiceProvider.GetRequiredService<DiscordDataContext>();
        SocketGuild? guild = _bot.GetGuild(_bot.GuildId);
        SocketGuildUser? discordMember = guild.GetUser(member.DiscordId.Value);
        List<ulong> rolesId = new();
        await foreach (Role role in data.Roles.ToAsyncEnumerable().OrderBy(x => x).ConfigureAwait(false))
            switch (role.Scope)
            {
                case DiscordRoleScope.Member:
                    if (member.Rank >= Rank.Neophyte) rolesId.Add(role.DiscordId);
                    break;
                case DiscordRoleScope.SquadPart:
                    if (member.SquadNumber == role.SquadNumber
                        && member.SquadPartNumber == role.SquadPartNumber
                        && member.Rank >= Rank.Auxiliary)
                        rolesId.Add(role.DiscordId);
                    break;
                case DiscordRoleScope.Squad:
                    if (member.SquadNumber == role.SquadNumber
                        && member.Rank >= Rank.Neophyte)
                        rolesId.Add(role.DiscordId);
                    break;
                case DiscordRoleScope.Direction:
                    if (member.Direction == role.Direction
                        && member.Rank >= Rank.Neophyte)
                        rolesId.Add(role.DiscordId);
                    break;
                case DiscordRoleScope.Advisor:
                    if (member.Rank >= Rank.Advisor) rolesId.Add(role.DiscordId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(role.Scope));
            }

        await discordMember.ModifyAsync(properties =>
        {
            properties.Nickname = member.ToString();
            properties.RoleIds = rolesId;
        }).ConfigureAwait(false);
    }
}