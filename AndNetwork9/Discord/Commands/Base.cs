using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Discord.Services;
using AndNetwork9.Shared.Backend;
using Discord;
using Discord.Commands;

namespace AndNetwork9.Discord.Commands;

public class Base : ModuleBase<DiscordCommandContext>
{
    private protected static readonly CultureInfo RussianCulture;

    private protected readonly DiscordBot Bot;
    private protected readonly ClanDataContext Data;

    static Base()
    {
        CultureInfo culture = CultureInfo.CreateSpecificCulture("ru");
        culture.NumberFormat.CurrencySymbol = "SC";

        culture.NumberFormat.CurrencyDecimalSeparator = culture.NumberFormat.NumberDecimalSeparator =
            culture.NumberFormat.PercentDecimalSeparator = ".";
        culture.NumberFormat.CurrencyGroupSeparator = culture.NumberFormat.NumberGroupSeparator =
            culture.NumberFormat.PercentGroupSeparator = ",";

        RussianCulture = culture;
    }

    public Base(DiscordBot bot, ClanDataContext data)
    {
        Bot = bot;
        Data = data;
    }

    protected Shared.Member GetCaller()
    {
        return Data.Members.First(x => x.DiscordId == Context.User.Id);
    }


    protected override async Task<IUserMessage> ReplyAsync(string? message = null, bool isTTS = false,
        Embed? embed = null, RequestOptions? options = null,
        AllowedMentions? allowedMentions = null, MessageReference? messageReference = null,
        MessageComponent? component = null,
        ISticker[]? stickers = null,
        Embed[]? embeds = null) => await Context.Message
        .ReplyAsync(message, isTTS, embed, allowedMentions, options, component, stickers, embeds)
        .ConfigureAwait(false);

    protected async Task<IUserMessage> ReplyFileAsync(Stream stream, string fileName, string? messageText = null,
        bool isTts = false, Embed? embed = null, RequestOptions? options = null, bool spoiler = false,
        AllowedMentions? allowedMentions = null) => await Context.Message.Channel.SendFileAsync(stream,
        fileName,
        messageText,
        isTts,
        embed,
        options,
        spoiler,
        allowedMentions,
        new(Context.Message.Id, Context.Message.Channel.Id)).ConfigureAwait(false);
}