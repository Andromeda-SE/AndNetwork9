using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AndNetwork9.Discord.Commands;
using AndNetwork9.Discord.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Direction = Discord.Direction;
using IConnection = RabbitMQ.Client.IConnection;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Discord.Listeners
{
    public class UpdateUser : BaseRabbitListenerWithoutResponse<ulong>
    {
        private readonly DiscordBot _bot;
        private readonly IServiceScopeFactory _scopeFactory;
        
        public IRole EveryoneRole { get; private set; } = null!;
        public IRole DefaultRole { get; private set; } = null!;
        public IRole AdvisorRole { get; private set; } = null!;
        public IRole FirstAdvisorRole { get; private set; } = null!;

        public ImmutableDictionary<Shared.Enums.Direction, IRole> DirectionRoles { get; private set; } = null!;

        public UpdateUser(IConnection connection, DiscordBot bot, ILogger<UpdateUser> logger, IServiceScopeFactory scopeFactory) : base(connection,
            UpdateUserSender.QUEUE_NAME,
            logger)
        {
            _bot = bot;
            _scopeFactory = scopeFactory;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _bot.Connected += BotOnConnected;
            await base.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task BotOnConnected()
        {
            _bot.Connected -= BotOnConnected;
            AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
            ClanDataContext? data = scope.ServiceProvider.GetService<ClanDataContext>();
            if (data is null) throw new();

            await InitRoles(await _bot.Rest.GetGuildAsync(_bot.GuildId).ConfigureAwait(false), data).ConfigureAwait(false);
        }

        public override async Task Run(ulong arg)
        {
            AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
            ClanDataContext? data = scope.ServiceProvider.GetService<ClanDataContext>();
            if (data is null) throw new();

            Shared.Member? clanMember = await data.Members.FirstOrDefaultAsync(x => x.DiscordId == arg).ConfigureAwait(false);
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

            List<ulong> rolesId = new ();

            if (discordMember.Hierarchy == int.MaxValue) rolesId.Add(FirstAdvisorRole.Id);
            if (clanMember.Direction > Shared.Enums.Direction.None) rolesId.Add(DirectionRoles[clanMember.Direction].Id);
            var guild = await _bot.Rest.GetGuildAsync(_bot.GuildId).ConfigureAwait(false);
            RestRole[] roles = discordMember.RoleIds.Select(x => guild.GetRole(x)).ToArray();
            if (roles.Any(x => x.Tags is not null && x.Tags.IsPremiumSubscriberRole)) rolesId.Add(roles.First(x => x.Tags.IsPremiumSubscriberRole).Id);
            if (clanMember.Rank >= Rank.Advisor) rolesId.Add(AdvisorRole.Id);
            if (clanMember.Rank > Rank.None) rolesId.Add(DefaultRole.Id);

            if (clanMember.Squad is not null)
            {
                if (clanMember.Squad.Part != 0)
                {
                    Squad? headSquad = await data.Squads.FindAsync(clanMember.Squad.Number, 0).ConfigureAwait(false);
                    if (headSquad?.DiscordRoleId is not null)
                    {
                        rolesId.Add(headSquad.DiscordRoleId.Value);
                    }
                }
                if (clanMember.Squad.DiscordRoleId is not null)
                {
                    rolesId.Add(clanMember.Squad.DiscordRoleId.Value);
                }
            }

            await discordMember.ModifyAsync(properties =>
            {
                if (discordMember.Hierarchy < int.MaxValue)
                {
                    properties.Nickname = clanMember.Rank > Rank.None ? clanMember.ToString() : null;
                }
                properties.RoleIds = rolesId.ToArray();
            }).ConfigureAwait(false);
        }

        private async Task InitRoles(IGuild guild, ClanDataContext data)
        {
            EveryoneRole = guild.EveryoneRole;
            IRole[] roles = guild.Roles.ToArray();

            string firstAdvisorName = Enum.GetValues<Rank>().Max().GetRankName();
            FirstAdvisorRole = roles.FirstOrDefault(x => x.Name == firstAdvisorName)
                               ?? await guild.CreateRoleAsync(firstAdvisorName,
                                   GuildPermissions.None,
                                   Color.Red,
                                   true,
                                   true,
                                   RequestOptions.Default).ConfigureAwait(false);

            ImmutableDictionary<Shared.Enums.Direction, IRole>.Builder rolesBuilder =
                ImmutableDictionary<Shared.Enums.Direction, IRole>.Empty.ToBuilder();
            foreach (Shared.Enums.Direction department in Enum.GetValues<Shared.Enums.Direction>().Where(x => x != Shared.Enums.Direction.None))
            {
                string name = department.GetName();
                IRole? role = roles.FirstOrDefault(x => x.Name == name)
                              ?? await guild.CreateRoleAsync(name,
                                  GuildPermissions.None,
                                  department.GetDiscordColor(),
                                  false,
                                  true,
                                  RequestOptions.Default).ConfigureAwait(false);
                rolesBuilder.Add(department, role);
            }

            DirectionRoles = rolesBuilder.ToImmutable();

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            ImmutableDictionary<Squad, IRole>.Builder squadRolesBuilder =
                ImmutableDictionary<Squad, IRole>.Empty.ToBuilder();
            foreach (Squad squad in data.Squads.Where(x => x.DisbandDate <= today))
            {
                IRole role = squad.DiscordRoleId is null
                    ? await guild.CreateRoleAsync(squad.ToString(),
                        GuildPermissions.None,
                        Color.Default,
                        true,
                        true,
                        RequestOptions.Default).ConfigureAwait(false)
                    : guild.Roles.First(x => x.Id == squad.DiscordRoleId.Value);
                squadRolesBuilder.Add(squad, role);
            }

            const string advisorRoleName = "Советник клана";
            AdvisorRole = roles.FirstOrDefault(x => x.Name == advisorRoleName)
                          ?? await guild.CreateRoleAsync(advisorRoleName,
                              GuildPermissions.None,
                              null,
                              false,
                              false,
                              RequestOptions.Default).ConfigureAwait(false);

            const string defaultRoleName = "Участник клана";
            DefaultRole = roles.FirstOrDefault(x => x.Name == defaultRoleName)
                          ?? await guild.CreateRoleAsync(defaultRoleName,
                              GuildPermissions.None,
                              null,
                              false,
                              false,
                              RequestOptions.Default).ConfigureAwait(false);
        }
    }
}