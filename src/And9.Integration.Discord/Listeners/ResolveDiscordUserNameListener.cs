using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using Discord;
using Discord.WebSocket;
using IConnection = RabbitMQ.Client.IConnection;

namespace And9.Integration.Discord.Listeners;

public class ResolveDiscordUserNameListener : BaseRabbitListenerWithResponse<string, ulong?>
{
    private readonly DiscordBot _bot;

    private SocketGuild _guild;

    public ResolveDiscordUserNameListener(IConnection connection, ILogger<BaseRabbitListenerWithResponse<string, ulong?>> logger, DiscordBot bot)
        : base(connection, ResolveDiscordUserNameSender.QUEUE_NAME, logger)
    {
        _bot = bot;
        _guild ??= _bot.GetGuild(_bot.GuildId);
    }

    protected override async Task<ulong?> GetResponseAsync(string request)
    {
        _guild ??= _bot.GetGuild(_bot.GuildId);
        request = request.Trim();
        if (string.IsNullOrWhiteSpace(request)) return default;
        string[] raw = request.Split('#', 2);
        if (raw.Length != 2) return default;
        IUser? user = await _guild.GetUsersAsync().Flatten()
            .FirstAsync(x => x.Username == raw[0] && x.Discriminator == raw[1]).ConfigureAwait(false);
        return user?.Id ?? default(ulong?);
    }
}