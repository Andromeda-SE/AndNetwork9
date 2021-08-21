using System.Threading.Tasks;
using AndNetwork9.Discord.Permissions;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Enums;
using Discord.Commands;

namespace AndNetwork9.Discord.Commands
{
    [Group(nameof(Admin))]
    public class Admin : Base
    {
        public Admin(DiscordBot bot, ClanDataContext data) : base(bot, data) { }

        [Command(nameof(Sync))]
        [Alias("update")]
        [Summary("Применяет изменения на сервере Discord")]
        [MinRankPermission(Rank.Advisor)]
        public async Task Sync()
        {
            await Bot.UpdateAsync().ConfigureAwait(false);
            await ReplyAsync("Обновление завершено").ConfigureAwait(false);
        }
    }
}