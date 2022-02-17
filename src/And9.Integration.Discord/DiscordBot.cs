using System.ComponentModel;
using System.Runtime.CompilerServices;
using And9.Integration.Discord.Extensions;
using Discord;
using Discord.WebSocket;

namespace And9.Integration.Discord;

public class DiscordBot : DiscordSocketClient, IHostedService
{
    public readonly ulong GuildId;
    internal readonly IServiceScopeFactory ScopeFactory;
    protected readonly string Token;

    [Localizable(false)]
    public DiscordBot(ILogger<DiscordBot> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory) :
        base(new()
        {
            LogLevel = LogSeverity.Info,
            DefaultRetryMode = RetryMode.AlwaysRetry,
            LargeThreshold = 250,
            UseSystemClock = true,
            AlwaysDownloadUsers = true,
            ConnectionTimeout = 30000,
        })
    {
        Log += OnLog;
        Logger = logger;
        Token = configuration["Discord:Token"];
        GuildId = ulong.Parse(configuration["Discord:Id"]);
        ScopeFactory = scopeFactory;
    }

    internal ILogger<DiscordBot> Logger { get; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        UserJoined += OnUserJoined;
        await LoginAsync(TokenType.Bot, Token).ConfigureAwait(false);
        await base.StartAsync().ConfigureAwait(false);
        Status = UserStatus.Online;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Status = UserStatus.Offline;
        await base.StopAsync().ConfigureAwait(false);
        await LogoutAsync().ConfigureAwait(false);
    }

    private async Task OnLog(LogMessage message)
    {
        await Task.Run(() =>
        {
            if (message.Exception is null) Logger.Log(message.Severity.ToLogLevel(), message.Message);
            else Logger.Log(message.Severity.ToLogLevel(), message.Exception, message.Message);
        }).ConfigureAwait(false);
    }


    private async Task OnUserJoined(SocketGuildUser user)
    {
        if (user.Guild.Id == GuildId)
        {
            AsyncServiceScope scope = ScopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
            //TODO send to Core service
        }
    }
}