using System.ComponentModel;
using System.Runtime.CompilerServices;
using And9.Integration.Discord.Extensions;
using And9.Integration.Discord.Senders;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders;
using Discord;
using Discord.WebSocket;
using Direction = And9.Service.Core.Abstractions.Enums.Direction;

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
        if (user.Guild.Id != GuildId) return;
        AsyncServiceScope scope = ScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        MemberCrudSender memberCrudSender = scope.ServiceProvider.GetRequiredService<MemberCrudSender>();
        Member? member = await memberCrudSender.ReadByDiscordId(user.Id).ConfigureAwait(false);
        if (member is null)
        {
            int id = await memberCrudSender.Create(new()
            {
                Direction = Direction.None,
                Rank = Rank.Guest,
                TimeZone = null,
                DiscordId = user.Id,
                ConcurrencyToken = Guid.NewGuid(),
                Nickname = user.Nickname,
                IsSquadCommander = false,
                LastChanged = DateTime.UtcNow,
                RealName = null,
                SquadPartNumber = 0,
                JoinDate = DateOnly.MinValue,
                MicrosoftId = null,
                LastDirectionChange = DateOnly.MinValue,
                SquadNumber = null,
                SteamId = null,
                TelegramId = null,
                VkId = null,
            }).ConfigureAwait(false);
            member = await memberCrudSender.Read(id).ConfigureAwait(false);
            if (member is null)
            {
                Logger.LogError("member not found after creation");
                return;
            }
        }

        SyncUserSender syncUserSender = scope.ServiceProvider.GetRequiredService<SyncUserSender>();
        await syncUserSender.CallAsync(member).ConfigureAwait(false);
    }
}