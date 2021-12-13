using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AndNetwork9.Discord.Services;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using Discord.Rest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Discord.Listeners;

public class UpdateUser : BaseRabbitListenerWithoutResponse<ulong>
{
    private readonly DiscordBot _bot;
    private readonly RoleManager _roleManager;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly UpdateUserSender _updateUserSender;

    public UpdateUser(IConnection connection, DiscordBot bot, ILogger<UpdateUser> logger,
        IServiceScopeFactory scopeFactory, RoleManager roleManager, UpdateUserSender updateUserSender) : base(
        connection,
        UpdateUserSender.QUEUE_NAME,
        logger)
    {
        _bot = bot;
        _scopeFactory = scopeFactory;
        _roleManager = roleManager;
        _updateUserSender = updateUserSender;
        _roleManager.Initialized += BotOnConnected;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task BotOnConnected()
    {
        _bot.Connected -= BotOnConnected;
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        ClanDataContext data = scope.ServiceProvider.GetRequiredService<ClanDataContext>();

        await foreach (Member member in data.Members.Where(x => x.DiscordId.HasValue).ToAsyncEnumerable()
                           .ConfigureAwait(false))
            await _updateUserSender.CallAsync(member.DiscordId!.Value).ConfigureAwait(false);
        _bot.Connected += BotOnConnected;
    }

    public override async Task Run(ulong arg)
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        ClanDataContext? data = scope.ServiceProvider.GetService<ClanDataContext>();
        if (data is null) throw new();

        Member? clanMember = await data.Members.FirstOrDefaultAsync(x => x.DiscordId == arg).ConfigureAwait(false);
        if (clanMember is null)
        {
            Logger.LogWarning($"Member with DiscordId = {arg} not found");
            return;
        }

        RestGuildUser? discordMember = await _bot.Rest.GetGuildUserAsync(_bot.GuildId, arg).ConfigureAwait(false);
        if (discordMember is null)
        {
            Logger.LogWarning($"Member {clanMember.Id}:«{clanMember.Nickname}» is not a member of Discord server");
            return;
        }

        List<ulong> rolesId = new();

        if (discordMember.Hierarchy == int.MaxValue) rolesId.Add(_roleManager.FirstAdvisorRole.Id);
        if (clanMember.Direction > Direction.None)
            rolesId.Add(_roleManager.DirectionRoles[clanMember.Direction].Id);
        RestGuild? guild = await _bot.Rest.GetGuildAsync(_bot.GuildId).ConfigureAwait(false);
        RestRole[] roles = discordMember.RoleIds.Select(x => guild.GetRole(x)).ToArray();
        if (roles.Any(x => x.Tags is not null && x.Tags.IsPremiumSubscriberRole))
            rolesId.Add(roles.First(x => x.Tags.IsPremiumSubscriberRole).Id);
        if (clanMember.Rank >= Rank.Advisor) rolesId.Add(_roleManager.AdvisorRole.Id);
        if (clanMember.Rank > Rank.None) rolesId.Add(_roleManager.DefaultRole.Id);

        if (clanMember.SquadPart is not null)
        {
            if (clanMember.SquadPart.DiscordRoleId.HasValue) rolesId.Add(clanMember.SquadPart.DiscordRoleId.Value);
            if (clanMember.SquadPart.Squad.DiscordRoleId.HasValue)
                rolesId.Add(clanMember.SquadPart.Squad.DiscordRoleId.Value);
        }

        if (discordMember.Hierarchy < int.MaxValue)
            await discordMember.ModifyAsync(properties =>
            {
                properties.Nickname = clanMember.Rank > Rank.None ? clanMember.ToString() : null;

                properties.RoleIds = rolesId.ToArray();
            }).ConfigureAwait(false);
    }
}