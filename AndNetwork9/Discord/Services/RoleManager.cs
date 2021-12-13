using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AndNetwork9.Discord.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using Discord;
using Discord.Rest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Direction = AndNetwork9.Shared.Enums.Direction;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Discord.Services;

public class RoleManager : IDisposable
{
    private readonly DiscordBot _discordBot;
    private readonly ILogger<RoleManager> _logger;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly UpdateUserSender _updateUserSender;

    public RoleManager(IServiceScopeFactory scopeFactory, DiscordBot discordBot, ILogger<RoleManager> logger,
        UpdateUserSender updateUserSender)
    {
        _scopeFactory = scopeFactory;
        _discordBot = discordBot;
        _logger = logger;
        _updateUserSender = updateUserSender;
        _discordBot.Connected += InitRoles;
        if (_discordBot.ConnectionState == ConnectionState.Connected)
        {
            Task init = InitRoles();
            init.Start();
            init.Wait();
        }
    }

    public IRole EveryoneRole { get; private set; } = null!;
    public IRole DefaultRole { get; private set; } = null!;
    public IRole AdvisorRole { get; private set; } = null!;
    public IRole FirstAdvisorRole { get; private set; } = null!;

    public ImmutableDictionary<Direction, IRole> DirectionRoles { get; private set; } = null!;

    public void Dispose()
    {
        _discordBot.Connected -= InitRoles;
    }

    internal event Func<Task> Initialized;

    private async Task InitRoles()
    {
        while (_discordBot.ConnectionState != ConnectionState.Connected) await Task.Delay(30).ConfigureAwait(true);
        try
        {
            AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
            ClanDataContext data = scope.ServiceProvider.GetRequiredService<ClanDataContext>();
            RestGuild guild = await _discordBot.Rest.GetGuildAsync(_discordBot.GuildId).ConfigureAwait(false);

            List<IRole> rolePriority = new(16);

            EveryoneRole = guild.EveryoneRole;
            RestRole[] roles = guild.Roles.ToArray();

            string firstAdvisorName = Enum.GetValues<Rank>().Max().GetRankName();
            FirstAdvisorRole = roles.FirstOrDefault(x => x.Name == firstAdvisorName)
                               ?? await guild.CreateRoleAsync(firstAdvisorName,
                                   GuildPermissions.None,
                                   Color.Red,
                                   true,
                                   true,
                                   RequestOptions.Default).ConfigureAwait(false);
            rolePriority.Add(FirstAdvisorRole);

            ImmutableDictionary<Direction, IRole>.Builder rolesBuilder =
                ImmutableDictionary<Direction, IRole>.Empty.ToBuilder();
            foreach (Direction department in Enum.GetValues<Direction>().Where(x => x != Direction.None))
            {
                string name = department.GetName();
                IRole? role = roles.FirstOrDefault(x => x.Name == name)
                              ?? (IRole)await guild.CreateRoleAsync(name,
                                  GuildPermissions.None,
                                  department.GetDiscordColor(),
                                  false,
                                  true,
                                  RequestOptions.Default).ConfigureAwait(false);
                rolePriority.Add(role);
                rolesBuilder.Add(department, role);
            }

            DirectionRoles = rolesBuilder.ToImmutable();

            foreach (Squad squad in data.Squads.ToArray())
            {
                IRole role = squad.DiscordRoleId is null
                    ? await guild.CreateRoleAsync(squad.ToString(),
                        GuildPermissions.None,
                        Color.Default,
                        true,
                        true,
                        RequestOptions.Default).ConfigureAwait(false)
                    : guild.Roles.First(x => x.Id == squad.DiscordRoleId.Value);
                squad.DiscordRoleId ??= role.Id;
                rolePriority.Add(role);
                foreach (SquadPart squadPart in squad.SquadParts)
                {
                    IRole partRole = squadPart.DiscordRoleId is null
                        ? await guild.CreateRoleAsync(squadPart.ToString(),
                            GuildPermissions.None,
                            Color.Default,
                            false,
                            true,
                            RequestOptions.Default).ConfigureAwait(false)
                        : guild.Roles.First(x => x.Id == squadPart.DiscordRoleId.Value);
                    squadPart.DiscordRoleId ??= partRole.Id;
                    rolePriority.Add(partRole);
                }
            }


            const string advisorRoleName = "Советник клана";
            AdvisorRole = roles.FirstOrDefault(x => x.Name == advisorRoleName)
                          ?? (IRole)await guild.CreateRoleAsync(advisorRoleName,
                              GuildPermissions.None,
                              null,
                              false,
                              false,
                              RequestOptions.Default).ConfigureAwait(false);
            rolePriority.Add(AdvisorRole);

            const string defaultRoleName = "Участник клана";
            DefaultRole = roles.FirstOrDefault(x => x.Name == defaultRoleName)
                          ?? (IRole)await guild.CreateRoleAsync(defaultRoleName,
                              GuildPermissions.None,
                              null,
                              false,
                              false,
                              RequestOptions.Default).ConfigureAwait(false);
            rolePriority.Add(DefaultRole);

            for (int i = 0; i < rolePriority.Count; i++)
            {
                int pos = rolePriority.Count - i;
                await rolePriority[i].ModifyAsync(properties => { properties.Position = pos; }).ConfigureAwait(false);
            }

            foreach (Member member in data.Members.Where(x => x.DiscordId.HasValue))
                await _updateUserSender.CallAsync(member.DiscordId!.Value).ConfigureAwait(false);

            await data.SaveChangesAsync().ConfigureAwait(false);
            Initialized?.Invoke();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error on init roles");
        }
    }
}