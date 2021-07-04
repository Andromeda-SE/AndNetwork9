using System;
using System.Threading.Tasks;
using AndNetwork9.Discord.Permissions;
using AndNetwork9.Shared.Backend;
using Discord.Commands;

namespace AndNetwork9.Discord.Commands
{
    public class Root : Base
    {
        private static readonly string HelpText = System.IO.File.ReadAllText("help.md");

        public Root(DiscordBot bot, ClanDataContext data) : base(bot, data) { }

        [Command(nameof(Time))]
        [Summary("Возвращает текущее время сервера")]
        public async Task Time()
        {
            await ReplyAsync(DateTime.Now.ToString("F", RussianCulture)).ConfigureAwait(false);
        }

        [Command(nameof(Help))]
        [Alias("?")]
        [Summary("Возвращает справку по всем командам")]
        [MinRankPermission]
        public async Task Help()
        {
            await ReplyAsync(HelpText).ConfigureAwait(false);
        }
    }
}