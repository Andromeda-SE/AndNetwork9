using System;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AndNetwork9.Discord.Permissions;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Storage;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndNetwork9.Discord.Commands
{
    [Group(nameof(File))]
    public class File : Base
    {
        public File(DiscordBot bot, ClanDataContext data) : base(bot, data) { }


        [Command(nameof(Upload))]
        [MinRankPermission]
        public async Task Upload()
        {
            StringBuilder text = new();
            foreach (Attachment attachment in Context.Message.Attachments)
            {
                string[] name = attachment.Filename.Split('.', 2, StringSplitOptions.RemoveEmptyEntries);
                EntityEntry<StaticFile> result = await Data.StaticFiles.AddAsync(new()
                {
                    Name = name[0],
                    Extension = name.Length >= 2 ? name[1] : string.Empty,
                    Path = attachment.Url,
                    ReadRule = new()
                    {
                        MinRank = Rank.Neophyte,
                    },
                }).ConfigureAwait(false);
                await Data.SaveChangesAsync().ConfigureAwait(false);
                text.AppendLine(
                    $"Файл {attachment.Filename} загружен и доступен по идентификатору \"{result.Entity.Id}\"");
            }

            await ReplyAsync(text.ToString()).ConfigureAwait(false);
        }

        [Command(nameof(Download))]
        public async Task Download(int fileId)
        {
            StaticFile? file = await Data.StaticFiles.FindAsync(fileId).ConfigureAwait(false);
            if (file is null)
            {
                await ReplyAsync("Файл не найден").ConfigureAwait(false);
                return;
            }

            if (!file.ReadRule.HasAccess(GetCaller()))
                await ReplyAsync("Доступ к файлу запрещен").ConfigureAwait(false);
            using HttpClient http = new();
            Stream fileStream = await http.GetStreamAsync(file.Path).ConfigureAwait(false);
            await using ConfiguredAsyncDisposable _ = fileStream.ConfigureAwait(false);
            await ReplyFileAsync(fileStream, file.ToString()).ConfigureAwait(false);
        }
    }
}