using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.Database.Models;
using And9.Integration.Discord.Extensions;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Models;
using Discord;
using Discord.WebSocket;
using Direction = And9.Service.Core.Abstractions.Enums.Direction;
using IConnection = RabbitMQ.Client.IConnection;

namespace And9.Integration.Discord.Listeners;

public class SyncChannelsListeners : BaseRabbitListenerWithoutResponse<IReadOnlyCollection<Member>>
{
    private readonly DiscordDataContext _discordDataContext;
    private readonly DiscordBot _bot;

    public SyncChannelsListeners(
        IConnection connection,
        ILogger<BaseRabbitListenerWithoutResponse<IReadOnlyCollection<Member>>> logger,
        DiscordDataContext discordDataContext,
        DiscordBot bot)
        : base(connection, SyncChannelsSender.QUEUE_NAME, logger)
    {
        _discordDataContext = discordDataContext;
        _bot = bot;
    }

    public override async Task Run(IReadOnlyCollection<Member> members)
    {
        SocketGuild? guild = _bot.GetGuild(_bot.GuildId);

        foreach (ChannelCategory channelCategory in _discordDataContext.ChannelCategories)
        {
            SocketCategoryChannel? discordCategory = guild.GetCategoryChannel(channelCategory.DiscordId);

            await discordCategory.ModifyAsync(properties =>
            {
                properties.Position = channelCategory.Position;
                properties.Name = channelCategory.Name;
            }).ConfigureAwait(false);
        }

        ulong everyoneRoleId = guild.EveryoneRole.Id;
        ulong memberRoleId = _discordDataContext.Roles.First(x => x.Scope == DiscordRoleScope.Member).DiscordId;
        ulong advisorRoleId = _discordDataContext.Roles.First(x => x.Scope == DiscordRoleScope.Advisor).DiscordId;
        Dictionary<short, ulong> squadRoleIds = _discordDataContext.Roles
            .Where(x => x.Scope == DiscordRoleScope.Squad)
            .ToDictionary(x => x.SquadNumber ?? default, role => role.DiscordId);
        Dictionary<(short SquadNumber, short SquadPartNumber), ulong> squadPartRoleIds = _discordDataContext.Roles
            .Where(x => x.Scope == DiscordRoleScope.SquadPart)
            .ToDictionary(x => (x.SquadNumber ?? default, x.SquadPartNumber ?? default), role => role.DiscordId);

        foreach (Channel channel in _discordDataContext.Channels)
        {
            SocketGuildChannel? discordChannel = channel.Type switch
            {
                DiscordChannelType.Text => guild.GetTextChannel(channel.DiscordId),
                DiscordChannelType.Voice => guild.GetVoiceChannel(channel.DiscordId),
                DiscordChannelType.Announcement => guild.GetTextChannel(channel.DiscordId),
                DiscordChannelType.Stage => guild.GetStageChannel(channel.DiscordId),
                DiscordChannelType.Thread => guild.GetThreadChannel(channel.DiscordId),
                _ => throw new ArgumentOutOfRangeException(),
            };

            List<Overwrite> overwrites = new(8)
            {
                new(everyoneRoleId,
                    PermissionTarget.Role,
                    channel.EveryonePermissions.ToOverwritePermissions()),
                new(memberRoleId,
                    PermissionTarget.Role,
                    channel.MemberPermissions.ToOverwritePermissions()),
                new(advisorRoleId,
                    PermissionTarget.Role,
                    channel.AdvisorPermissions.ToOverwritePermissions()),
            };
            if (channel.SquadNumber is not null)
            {
                overwrites.Add(new(squadRoleIds[channel.SquadNumber.Value],
                    PermissionTarget.Role,
                    channel.SquadPermissions.ToOverwritePermissions()));
                ulong? squadCommander = members.FirstOrDefault(x => x.IsSquadCommander && x.SquadNumber == 0)?.DiscordId;
                if (squadCommander is not null)
                    overwrites.Add(new(squadCommander.Value,
                        PermissionTarget.User,
                        channel.SquadCaptainPermissions.ToOverwritePermissions()));
                overwrites.AddRange(members.Where(x => x.IsSquadCommander && x.SquadNumber != 0 && x.DiscordId is not null)
                    .Select(lieutenant => new Overwrite(lieutenant.DiscordId.Value,
                        PermissionTarget.User,
                        channel.SquadLieutenantsPermissions.ToOverwritePermissions())));
                if (channel.SquadPartNumber is not null)
                    overwrites.Add(new(squadPartRoleIds[(channel.SquadNumber.Value, channel.SquadPartNumber.Value)],
                        PermissionTarget.Role,
                        channel.SquadPartPermissions.ToOverwritePermissions()));
            }

            ChannelCategory? category = await _discordDataContext.ChannelCategories.FindAsync(channel.CategoryId).ConfigureAwait(false);
            await discordChannel.ModifyAsync(properties =>
            {
                properties.Name = channel.Name;
                properties.Position = channel.ChannelPosition;
                properties.PermissionOverwrites = overwrites;
                properties.CategoryId = category?.DiscordId;
            }).ConfigureAwait(false);
        }
    }
}