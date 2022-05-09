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

public class DiscordBot : DiscordSocketClient, IHostedService, IAsyncDisposable
{
    private readonly ReadMemberByDiscordIdSender _readMemberByDiscordIdSender;
    private readonly CreateMemberSender _createMemberSender;
    private readonly ReadMemberByIdSender _readMemberByIdSender;
    private readonly SyncUserSender _syncUserSender;
    private readonly ConfiguredAsyncDisposable _configuredAsyncDisposable;
    public readonly ulong GuildId;
    protected readonly string Token;

    [Localizable(false)]
    public DiscordBot(
        ILogger<DiscordBot> logger,
        IConfiguration configuration, 
        IServiceScopeFactory serviceScopeFactory) :
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
        AsyncServiceScope scope = serviceScopeFactory.CreateAsyncScope();
        _configuredAsyncDisposable = scope.ConfigureAwait(false);
        _readMemberByDiscordIdSender = scope.ServiceProvider.GetRequiredService<ReadMemberByDiscordIdSender>();
        _createMemberSender = scope.ServiceProvider.GetRequiredService<CreateMemberSender>();
        _readMemberByIdSender = scope.ServiceProvider.GetRequiredService<ReadMemberByIdSender>();
        _syncUserSender = scope.ServiceProvider.GetRequiredService<SyncUserSender>();
        Token = configuration["Discord:Token"];
        GuildId = ulong.Parse(configuration["Discord:Id"]);
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

        Member? member = await _readMemberByDiscordIdSender.CallAsync(user.Id).ConfigureAwait(false);
        if (member is null)
        {
            int id = await _createMemberSender.CallAsync(new()
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
            member = await _readMemberByIdSender.CallAsync(id).ConfigureAwait(false);
            if (member is null)
            {
                Logger.LogError("member not found after creation");
                return;
            }
        }

        await _syncUserSender.CallAsync(member).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync() => await _configuredAsyncDisposable.DisposeAsync();
}