using System.Runtime.CompilerServices;
using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Abstractions.Models;
using And9.Integration.Discord.Database;
using And9.Integration.Discord.Extensions;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders;
using Discord;
using Discord.WebSocket;
using IConnection = RabbitMQ.Client.IConnection;

namespace And9.Integration.Discord.Listeners;

public class SyncChannelsListener : BaseRabbitListenerWithoutResponse<object>
{
    private readonly DiscordBot _bot;
    private readonly MemberCrudSender _memberCrudSender;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SyncChannelsListener(
        IConnection connection,
        ILogger<SyncChannelsListener> logger,
        DiscordBot bot,
        MemberCrudSender memberCrudSender, IServiceScopeFactory serviceScopeFactory)
        : base(connection, SyncChannelsSender.QUEUE_NAME, logger)
    {
        _bot = bot;
        _memberCrudSender = memberCrudSender;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task Run(object obj)
    {
        SocketGuild? guild = _bot.GetGuild(_bot.GuildId);
        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        DiscordDataContext discordDataContext = scope.ServiceProvider.GetRequiredService<DiscordDataContext>();

        foreach (ChannelCategory channelCategory in discordDataContext.ChannelCategories)
        {
            SocketCategoryChannel? discordCategory = guild.GetCategoryChannel(channelCategory.DiscordId);

            await discordCategory.ModifyAsync(properties =>
            {
                properties.Position = channelCategory.Position;
                properties.Name = channelCategory.Name;
            }).ConfigureAwait(false);
        }

        ulong everyoneRoleId = guild.EveryoneRole.Id;
        ulong memberRoleId = discordDataContext.Roles.First(x => x.Scope == DiscordRoleScope.Member).DiscordId;
        ulong advisorRoleId = discordDataContext.Roles.First(x => x.Scope == DiscordRoleScope.Advisor).DiscordId;
        Dictionary<short, ulong> squadRoleIds = discordDataContext.Roles
            .Where(x => x.Scope == DiscordRoleScope.Squad)
            .ToDictionary(x => x.SquadNumber ?? default, role => role.DiscordId);
        Dictionary<(short SquadNumber, short SquadPartNumber), ulong> squadPartRoleIds = discordDataContext.Roles
            .Where(x => x.Scope == DiscordRoleScope.SquadPart)
            .ToDictionary(x => (x.SquadNumber ?? default, x.SquadPartNumber ?? default), role => role.DiscordId);

        foreach (Channel channel in discordDataContext.Channels)
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
                Member[] members = await _memberCrudSender.ReadAll(CancellationToken.None).Where(x => x.SquadNumber == channel.SquadNumber).ToArrayAsync().ConfigureAwait(false);
                overwrites.Add(new(squadRoleIds[channel.SquadNumber.Value],
                    PermissionTarget.Role,
                    channel.SquadPermissions.ToOverwritePermissions()));
                ulong? squadCommander = members.FirstOrDefault(x => x.IsSquadCommander && x.SquadPartNumber == 0)?.DiscordId;
                if (squadCommander is not null)
                    overwrites.Add(new(squadCommander.Value,
                        PermissionTarget.User,
                        channel.SquadCaptainPermissions.ToOverwritePermissions()));
                ulong? squadPartCommander = members.FirstOrDefault(x => x.IsSquadCommander && x.SquadPartNumber == channel.SquadPartNumber)?.DiscordId;
                if (squadPartCommander is not null)
                    overwrites.Add(new(squadPartCommander.Value,
                        PermissionTarget.User,
                        channel.SquadCaptainPermissions.ToOverwritePermissions()));
                overwrites.AddRange(members.Where(x => x.IsSquadCommander && x.SquadPartNumber != 0 && x.DiscordId is not null)
                    .Select(lieutenant => new Overwrite(lieutenant.DiscordId!.Value,
                        PermissionTarget.User,
                        channel.SquadLieutenantsPermissions.ToOverwritePermissions())));
                if (channel.SquadPartNumber is not null)
                    overwrites.Add(new(squadPartRoleIds[(channel.SquadNumber.Value, channel.SquadPartNumber.Value)],
                        PermissionTarget.Role,
                        channel.SquadPartPermissions.ToOverwritePermissions()));
            }

            ChannelCategory? category = await discordDataContext.ChannelCategories.FindAsync(channel.CategoryId).ConfigureAwait(false);
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