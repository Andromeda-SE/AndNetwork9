using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndNetwork9.Discord.Comparers;
using AndNetwork9.Discord.Permissions;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Enums;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Direction = AndNetwork9.Shared.Enums.Direction;

namespace AndNetwork9.Discord.Commands
{
    [Group(nameof(Send))]
    public class Send : Base
    {
        private readonly ILogger<Send> _logger;

        public Send(DiscordBot bot, ClanDataContext data, ILogger<Send> logger) : base(bot, data) => _logger = logger;


        [Command(nameof(Dm))]
        [MinRankPermission(Rank.Advisor)]
        public async Task Dm(int id, string message)
        {
            Shared.Member? member = await Data.Members.FindAsync(id).ConfigureAwait(false);
            if (member is null)
            {
                await ReplyAsync("Игрок не найден").ConfigureAwait(false);
            }
            else
            {
                if (await TrySendMessage(message, member).ConfigureAwait(false))
                    await ReplyAsync($"Сообщение отправлено игроку <@{member.DiscordId}>").ConfigureAwait(false);
                else
                    await ReplyAsync($"Ошибка при отправке сообщения игроку <@{member.DiscordId}>")
                        .ConfigureAwait(false);
            }
        }

        [Command(nameof(All))]
        [MinRankPermission(Rank.Advisor)]
        public async Task All(string message)
        {
            await ReplyAsync(
                (await SendMessages(message, Data.Members.Where(x => x.Rank > Rank.None).AsAsyncEnumerable())
                    .ConfigureAwait(false))
                .ToString()).ConfigureAwait(false);
        }

        [Command(nameof(Online))]
        [MinRankPermission(Rank.Advisor)]
        public async Task Online(string message)
        {
            await ReplyAsync((await SendMessages(message,
                        Data.Members.Where(x => x.Rank > Rank.None).AsAsyncEnumerable().Where(x =>
                            Bot.GetUser(x.DiscordId).Status is > UserStatus.Offline and < UserStatus.DoNotDisturb))
                    .ConfigureAwait(false))
                .ToString()).ConfigureAwait(false);
        }

        [Command(nameof(Direction))]
        [MinRankPermission(Rank.Advisor)]
        public async Task Direction(Direction direction, string message)
        {
            await ReplyAsync((await SendMessages(message,
                        Data.Members.Where(x => x.Rank > Rank.None && x.Direction == direction).AsAsyncEnumerable())
                    .ConfigureAwait(false))
                .ToString()).ConfigureAwait(false);
        }

        [Command(nameof(Reaction))]
        [MinRankPermission(Rank.Advisor)]
        public async Task Reaction(ulong channelId, ulong messageId, string text)
        {
            SocketTextChannel channel = Bot.GetGuild(Bot.GuildId).GetTextChannel(channelId);
            if (channel is null)
            {
                await ReplyAsync("Канал не найден").ConfigureAwait(false);
                return;
            }

            IMessage message = await channel.GetMessageAsync(messageId).ConfigureAwait(false);
            if (message is null)
            {
                await ReplyAsync("Сообщение не найдено").ConfigureAwait(false);
                return;
            }

            IAsyncEnumerable<IUser> users = message.Reactions
                .SelectMany(x => message.GetReactionUsersAsync(x.Key, int.MaxValue).ToEnumerable()).SelectMany(x => x)
                .Distinct(DiscordIdComparer.Static).ToAsyncEnumerable();
            IAsyncEnumerable<Shared.Member> members = Data.Members.AsAsyncEnumerable()
                .Join(users, x => x.DiscordId, x => x.Id, (member, _) => member);
            await ReplyAsync((await SendMessages(text, members).ConfigureAwait(false)).ToString())
                .ConfigureAwait(false);
        }

        [Command(nameof(NoReaction))]
        [MinRankPermission(Rank.Advisor)]
        public async Task NoReaction(ulong channelId, ulong messageId, string text)
        {
            SocketTextChannel channel = Bot.GetGuild(Bot.GuildId).GetTextChannel(channelId);
            if (channel is null)
            {
                await ReplyAsync("Канал не найден").ConfigureAwait(false);
                return;
            }

            IMessage message = await channel.GetMessageAsync(messageId).ConfigureAwait(false);
            if (message is null)
            {
                await ReplyAsync("Сообщение не найдено").ConfigureAwait(false);
                return;
            }

            IAsyncEnumerable<IUser> users = message.Reactions
                .SelectMany(x => message.GetReactionUsersAsync(x.Key, int.MaxValue).ToEnumerable()).SelectMany(x => x)
                .Distinct(DiscordIdComparer.Static).ToAsyncEnumerable();
            users = channel.Users.ToAsyncEnumerable().Except(users, DiscordIdComparer.Static);
            IAsyncEnumerable<Shared.Member> members = Data.Members.AsAsyncEnumerable()
                .Join(users, x => x.DiscordId, x => x.Id, (member, _) => member);

            await ReplyAsync((await SendMessages(text, members).ConfigureAwait(false)).ToString())
                .ConfigureAwait(false);
        }

        private async Task<StringBuilder> SendMessages(string text, IAsyncEnumerable<Shared.Member> members,
            StringBuilder? log = null)
        {
            log ??= new();
            log.AppendLine("Отправлены сообщения:");
            await foreach (Shared.Member member in members.ConfigureAwait(false))
                if (await TrySendMessage(text, member).ConfigureAwait(false)) log.AppendLine($"<@{member.DiscordId}>");
                else log.AppendLine($"<@{member.DiscordId}> — ошибка");
            return log;
        }

        private async Task<bool> TrySendMessage(string text, Shared.Member member)
        {
            try
            {
                await Bot.GetUser(member.DiscordId).SendMessageAsync(text).ConfigureAwait(false);
                _logger.LogInformation($"Send message to {member} with id {member.Id}");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Error while sending message to {member} with id {member.Id}");
                return false;
            }
        }
    }
}