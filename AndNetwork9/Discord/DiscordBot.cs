using System;
using System.Threading;
using System.Threading.Tasks;
using AndNetwork9.Discord.Commands;
using AndNetwork9.Discord.Extensions;
using AndNetwork9.Shared.Backend;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IResult = Discord.Commands.IResult;

namespace AndNetwork9.Discord
{
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
                RateLimitPrecision = RateLimitPrecision.Millisecond,
                UseSystemClock = true,
                AlwaysDownloadUsers = true,
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
            await LoginAsync(TokenType.Bot, Token);
            await InstallCommandsAsync().ConfigureAwait(false);
            await base.StartAsync();
            Status = UserStatus.Online;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Status = UserStatus.Offline;
            await base.StopAsync();
            await UninstallCommandsAsync();
            await LogoutAsync();
        }

        private IDisposable CreateDatabaseConnection(out ClanDataContext data)
        {
            IServiceScope result = ScopeFactory.CreateScope();
            data = result.ServiceProvider.GetService(typeof(ClanDataContext)) as ClanDataContext
                   ?? throw new ArgumentException("ClanDataContext is null");
            return result;
        }

        public async Task UpdateAsync()
        {
            using IDisposable _ = CreateDatabaseConnection(out ClanDataContext data);
            await new DiscordUpdater(this, data).UpdateAsync();
        }

        private async Task OnLog(LogMessage message)
        {
            await Task.Run(() =>
            {
                if (message.Exception is null) Logger.Log(message.Severity.ToLogLevel(), message.Message);
                else Logger.Log(message.Severity.ToLogLevel(), message.Exception, message.Message);
            });
        }

        private Task OnUserJoined(SocketGuildUser user)
        {
            return Task.CompletedTask;
        }

        public async Task InstallCommandsAsync()
        {
            using IServiceScope scope = ScopeFactory.CreateScope();
            await _commandService.AddModuleAsync<Admin>(scope.ServiceProvider);
            await _commandService.AddModuleAsync<Member>(scope.ServiceProvider);
            await _commandService.AddModuleAsync<Elections>(scope.ServiceProvider);
            await _commandService.AddModuleAsync<Send>(scope.ServiceProvider);
            await _commandService.AddModuleAsync<Root>(scope.ServiceProvider);
            await _commandService.AddModuleAsync<Commands.File>(scope.ServiceProvider);

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
                await context.Message.ReplyAsync(result.Error?.GetLocalizedString() ?? "Неизвестная ошибка");
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
            foreach (ModuleInfo module in _commandService.Modules) await _commandService.RemoveModuleAsync(module);
        }

        private Task HandleCommandAsync(SocketMessage messageParam)
        {
            Task.Run(() => HandleCommand(messageParam));
            return Task.CompletedTask;
        }

        private async Task HandleCommand(SocketMessage messageParam)
        {
            if (messageParam is not SocketUserMessage message) return;
            int argPos = 0;
            if (message.Author.IsBot || !message.HasCharPrefix('/', ref argPos)) return;

            IServiceScope scope = ScopeFactory.CreateScope();
            IDisposable enterTypingState = message.Channel.EnterTypingState();
            DiscordCommandContext context = new(this, message, scope, enterTypingState);
            try
            {
                await _commandService.ExecuteAsync(context, argPos, scope.ServiceProvider);
            }
            catch (Exception e)
            {
                await message.Author.SendMessageAsync(
                    "Ошибка при обработке команды. Обратитесь к первому советнику клана");
                Logger.LogError(e, "Исключение при обработке сообщения от пользователя");
                enterTypingState.Dispose();
                scope.Dispose();
            }
        }

        public new async void Dispose()
        {
            await StopAsync();
            base.Dispose();
        }
    }
}