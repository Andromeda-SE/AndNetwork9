using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.Database.Models;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using Discord.WebSocket;

namespace And9.Integration.Discord.ConsumerStrategies;

[QueueName(SyncUserSender.QUEUE_NAME)]
public class SyncUserConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<Member>
{
    private readonly DiscordBot _bot;
    private readonly DiscordDataContext _discordDataContext;

    public SyncUserConsumerStrategy(DiscordBot bot, DiscordDataContext discordDataContext)
    {
        _bot = bot;
        _discordDataContext = discordDataContext;
    }

    public async ValueTask ExecuteAsync(Member member)
    {
        if (!member.DiscordId.HasValue) throw new ArgumentException("DiscordId is null", nameof(member));
        SocketGuild? guild = _bot.GetGuild(_bot.GuildId);
        SocketGuildUser? discordMember = guild.GetUser(member.DiscordId.Value);
        List<ulong> rolesId = new();
        await foreach (Role role in _discordDataContext.Roles.ToAsyncEnumerable().OrderBy(x => x).ConfigureAwait(false))
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