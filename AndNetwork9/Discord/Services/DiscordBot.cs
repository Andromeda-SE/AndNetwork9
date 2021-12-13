using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AndNetwork9.Discord.Commands;
using AndNetwork9.Discord.Extensions;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Enums;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AndNetwork9.Discord.Services;

public class DiscordBot : DiscordSocketClient, IHostedService
{
    private readonly CommandService _commandService = new(new()
    {
        CaseSensitiveCommands = false,
        DefaultRunMode = RunMode.Async,
        IgnoreExtraArgs = true,
        ThrowOnError = false,
        LogLevel = LogSeverity.Debug,
    });

    public readonly ulong GuildId;
    internal readonly IServiceScopeFactory ScopeFactory;
    protected readonly string Token;

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
        await InstallCommandsAsync().ConfigureAwait(false);
        await base.StartAsync().ConfigureAwait(false);
        Status = UserStatus.Online;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Status = UserStatus.Offline;
        await base.StopAsync().ConfigureAwait(false);
        await UninstallCommandsAsync().ConfigureAwait(false);
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
            ClanDataContext? data = scope.ServiceProvider.GetRequiredService<ClanDataContext>();

            if (!data.Members.Any(x => x.DiscordId == user.Id))
                data.Members.Add(new()
                {
                    DiscordId = user.Id,
                    JoinDate = DateOnly.FromDateTime(DateTime.Today),
                    Nickname = user.Nickname ?? user.Username,
                    Rank = Rank.Guest,
                });
        }
    }

    public async Task InstallCommandsAsync()
    {
        using IServiceScope scope = ScopeFactory.CreateScope();
        await _commandService.AddModuleAsync<Member>(scope.ServiceProvider).ConfigureAwait(false);
        await _commandService.AddModuleAsync<Elections>(scope.ServiceProvider).ConfigureAwait(false);
        await _commandService.AddModuleAsync<Root>(scope.ServiceProvider).ConfigureAwait(false);

        MessageReceived += HandleCommandAsync;
        _commandService.CommandExecuted += CommandExecuted;
    }

    private async Task CommandExecuted(Optional<CommandInfo> info, ICommandContext context, IResult result)
    {
        IUserMessage message = context.Message;
        using IDisposable loggerScope = Logger.BeginScope("Command processing");
        if (!result.IsSuccess)
        {
            Logger.LogInformation(
                $"{context.User} sent wrong command \"{message.Content}\": «{result.Error}/{result.ErrorReason}»");
            await context.Message.ReplyAsync(result.Error?.GetLocalizedString() ?? "Неизвестная ошибка")
                .ConfigureAwait(false);
            if (result is ExecuteResult executeResult)
                throw executeResult.Exception;
        }
        else
        {
            Logger.LogInformation($"{message.Author} sent \"{message.Content}\"");
        }

        if (context is DiscordCommandContext discordCommandContext) discordCommandContext.Dispose();
    }

    public async Task UninstallCommandsAsync()
    {
        MessageReceived -= HandleCommandAsync;
        foreach (ModuleInfo module in _commandService.Modules)
            await _commandService.RemoveModuleAsync(module).ConfigureAwait(false);
    }

    private Task HandleCommandAsync(SocketMessage messageParam)
    {
        Task.Run(async () => await ProcessCommand(messageParam).ConfigureAwait(false));
        return Task.CompletedTask;
    }

    private async Task ProcessCommand(SocketMessage messageParam)
    {
        if (messageParam is not SocketUserMessage message) return;
        int argPos = 0;
        if (message.Author.IsBot || !message.HasCharPrefix('/', ref argPos)) return;

        IServiceScope scope = ScopeFactory.CreateScope();
        IDisposable enterTypingState = message.Channel.EnterTypingState();
        DiscordCommandContext context = new(this, message, scope, enterTypingState);
        try
        {
            await _commandService.ExecuteAsync(context, argPos, scope.ServiceProvider).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await message.Author.SendMessageAsync(
                "Ошибка при обработке команды. Обратитесь к первому советнику клана").ConfigureAwait(false);
            Logger.LogError(e, "Исключение при обработке сообщения от пользователя");
        }
        finally
        {
            enterTypingState.Dispose();
            scope.Dispose();
        }
    }

    public new async void Dispose()
    {
        await StopAsync().ConfigureAwait(false);
        base.Dispose();
    }
}