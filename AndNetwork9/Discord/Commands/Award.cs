using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Discord.Permissions;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace AndNetwork9.Discord.Commands
{
    [Group(nameof(Award))]
    public class Award : Base
    {
        private readonly ILogger<Award> _logger;
        private readonly PublishSender _publishSender;

        public Award(DiscordBot bot, ClanDataContext data, ILogger<Award> logger, PublishSender publishSender) :
            base(bot, data)
        {
            _logger = logger;
            _publishSender = publishSender;
        }

        [Command(nameof(Rise))]
        [Summary("Выдает повышения согласно полученным наградам")]
        [MinRankPermission(Rank.Advisor)]
        public async Task Rise()
        {
            using IDisposable logScope = _logger.BeginScope(this);

            Dictionary<Shared.Member, Rank> rises = new();
            await foreach (Shared.Member member in Data.Members
                .Where(x => x.Direction > Direction.None && x.Rank < Rank.Advisor)
                .ToAsyncEnumerable())
            {
                Rank rank = member.Awards.GetRank();
                if (member.Rank >= rank) continue;
                member.Rank = rank;
                _logger.LogInformation($"Member «{member}» gets rank {member.Rank}");
                rises.Add(member, rank);
            }

            await Data.SaveChangesAsync().ConfigureAwait(true);
            await _publishSender.CallAsync(string.Join(Environment.NewLine,
                rises.Select(x => $"Игрок <@{x.Key.DiscordId:D}> повышен до ранга «{x.Value.GetRankName()}»")));
            await ReplyAsync("Готово");
        }
    }
}