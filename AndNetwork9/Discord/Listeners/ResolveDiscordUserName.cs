using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Discord.Services;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Discord;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IConnection = RabbitMQ.Client.IConnection;

namespace AndNetwork9.Discord.Listeners;

public class ResolveDiscordUserName : BaseRabbitListenerWithResponse<string, ulong?>
{
    private readonly DiscordBot _bot;
    private readonly IServiceScopeFactory _scopeFactory;

    private SocketGuild _guild;

    public ResolveDiscordUserName(IConnection connection,
        ILogger<BaseRabbitListenerWithResponse<string, ulong?>> logger,
        DiscordBot bot,
        IServiceScopeFactory scopeFactory) : base(connection, ResolveDiscordUserNameSender.QUEUE_NAME, logger)
    {
        _bot = bot;
        _guild ??= _bot.GetGuild(_bot.GuildId);

        _scopeFactory = scopeFactory;
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