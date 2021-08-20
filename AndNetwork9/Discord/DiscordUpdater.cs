using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AndNetwork9.Discord.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Discord.Channels;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Direction = AndNetwork9.Shared.Enums.Direction;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Discord
{
    internal sealed class DiscordUpdater
    {
        private readonly DiscordBot _bot;
        private readonly ClanDataContext _data;

        public DiscordUpdater(DiscordBot bot, ClanDataContext data)
        {
            _bot = bot;
            _data = data;
        }

        public ImmutableDictionary<Direction, IRole> DirectionRoles { get; private set; } = null!;
        public ImmutableDictionary<Squad, IRole> SquadsRoles { get; private set; } = null!;
        public IRole EveryoneRole { get; private set; } = null!;
        public IRole DefaultRole { get; private set; } = null!;
        public IRole AdvisorRole { get; private set; } = null!;
        public IRole FirstAdvisorRole { get; private set; } = null!;

        public async Task UpdateAsync()
        {
            SocketGuild guild = _bot.GetGuild(_bot.GuildId);

            await InitRoles(guild);

            await foreach (Member member in _data.Members.ToAsyncEnumerable()) await UpdateMember(guild, member);

            await foreach (Channel channel in _data.DiscordChannels.ToAsyncEnumerable())
                await UpdateChannel(guild, channel);
        }

        private async Task InitRoles(IGuild guild)
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
                                   RequestOptions.Default);

            ImmutableDictionary<Direction, IRole>.Builder rolesBuilder =
                ImmutableDictionary<Direction, IRole>.Empty.ToBuilder();
            foreach (Direction department in Enum.GetValues<Direction>().Where(x => x != Direction.None))
            {
                string name = department.GetName();
                IRole? role = roles.FirstOrDefault(x => x.Name == name)
                              ?? await guild.CreateRoleAsync(name,
                                  GuildPermissions.None,
                                  department.GetDiscordColor(),
                                  false,
                                  true,
                                  RequestOptions.Default);
                rolesBuilder.Add(department, role);
            }

            DirectionRoles = rolesBuilder.ToImmutable();

            ImmutableDictionary<Squad, IRole>.Builder squadRolesBuilder =
                ImmutableDictionary<Squad, IRole>.Empty.ToBuilder();
            foreach (Squad squad in await _data.Squads
                .Where(x => x.DisbandDate <= DateOnly.FromDateTime(DateTime.UtcNow)).ToArrayAsync())
            {
                IRole role = squad.DiscordRoleId is null
                    ? await guild.CreateRoleAsync(squad.ToString(),
                        GuildPermissions.None,
                        Color.Default,
                        true,
                        true,
                        RequestOptions.Default)
                    : guild.Roles.First(x => x.Id == squad.DiscordRoleId.Value);
                squadRolesBuilder.Add(squad, role);
            }

            SquadsRoles = squadRolesBuilder.ToImmutable();

            const string advisorRoleName = "Советник клана";
            AdvisorRole = roles.FirstOrDefault(x => x.Name == advisorRoleName)
                          ?? await guild.CreateRoleAsync(advisorRoleName,
                              GuildPermissions.None,
                              null,
                              false,
                              false,
                              RequestOptions.Default);

            const string defaultRoleName = "Участник клана";
            DefaultRole = roles.FirstOrDefault(x => x.Name == defaultRoleName)
                          ?? await guild.CreateRoleAsync(defaultRoleName,
                              GuildPermissions.None,
                              null,
                              false,
                              false,
                              RequestOptions.Default);
        }

        private async Task UpdateMember(SocketGuild guild, Member member)
        {
            SocketGuildUser? user = guild.Users.FirstOrDefault(x => x.Id == member.DiscordId);

            if (user is null) throw new ArgumentNullException(nameof(member));

            List<IRole> userRoles = user.Roles.Cast<IRole>().ToList();

            if (member.Rank > Rank.None)
            {
                List<IRole> validRoles = new()
                {
                    DefaultRole,
                };
                if (user.Hierarchy == int.MaxValue) validRoles.Add(FirstAdvisorRole);
                if (member.Rank >= Rank.Advisor) validRoles.Add(AdvisorRole);
                if (member.Direction != Direction.None) validRoles.Add(DirectionRoles[member.Direction]);

                List<IRole> invalidRoles = userRoles.Except(validRoles).ToList();
                validRoles = validRoles.Except(userRoles).ToList();

                invalidRoles.Remove(EveryoneRole);
                if (invalidRoles.Count > 0) await user.RemoveRolesAsync(invalidRoles);
                if (validRoles.Count > 0) await user.AddRolesAsync(validRoles);

                string nickname = member.ToString();
                if (user.Nickname != nickname)
                    await user.ModifyAsync(x => x.Nickname = nickname, RequestOptions.Default);
            }
            else if (!user.IsBot)
            {
                if (user.Nickname is not null)
                    await user.ModifyAsync(properties => properties.Nickname = Optional<string>.Unspecified,
                        RequestOptions.Default);
                userRoles.Remove(EveryoneRole);
                if (userRoles.Count > 0) await user.RemoveRolesAsync(userRoles);
            }
        }

        private async Task UpdateChannel(SocketGuild guild, Channel channel)
        {
            SocketGuildChannel discordChannel = guild.GetChannel(channel.DiscordId);
            await discordChannel.ModifyAsync(properties =>
            {
                properties.PermissionOverwrites = new(channel.ToOverwrites(this));
                properties.Position = new(channel.ChannelPosition);
                properties.CategoryId = channel.Category is null
                    ? Optional<ulong?>.Unspecified
                    : new(channel.Category.DiscordId);
                properties.Name = channel.Name;
            });
        }
    }
}