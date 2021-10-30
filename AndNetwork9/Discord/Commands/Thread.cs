using AndNetwork9.Discord.Services;
using AndNetwork9.Shared.Backend;
using Discord.Commands;

namespace AndNetwork9.Discord.Commands;

[Group(nameof(Thread))]
public class Thread : Base
{
    public Thread(DiscordBot bot, ClanDataContext data) : base(bot, data) { }

    /*[Command(nameof(Dm))]
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
    }*/
}