using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Discord.Channels;
using Discord;
using Discord.Rest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AndNetwork9.Discord.Services;

public class ThreadReviver : ITimerService
{
    private readonly DiscordBot _discordBot;
    private readonly ILogger<ThreadReviver> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public ThreadReviver(IConfiguration configuration, IServiceScopeFactory scopeFactory, DiscordBot discordBot,
        ILogger<ThreadReviver> logger)
    {
        _scopeFactory = scopeFactory;
        _discordBot = discordBot;
        _logger = logger;
        Interval = TimeSpan.Parse(configuration["THREAD_REVIVER_INTERVAL"]);
    }

    protected TimeSpan Interval { get; init; }

    CancellationTokenSource? ITimerService.CancellationTokenSource { get; set; }
    TimeSpan ITimerService.Interval => Interval;

    PeriodicTimer? ITimerService.Timer { get; set; }

    public async Task Process()
    {
        throw new NotImplementedException("Discord.Net cannot update threads");
        try
        {
            _logger.LogInformation("Triggering " + nameof(ThreadReviver));
            if (_discordBot.ConnectionState != ConnectionState.Connected)
            {
                _logger.LogWarning("Discord bot is not connected");
                return;
            }

            AsyncServiceScope serviceScope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = serviceScope.ConfigureAwait(false);
            ClanDataContext data = serviceScope.ServiceProvider.GetRequiredService<ClanDataContext>();
            int count = 0;
            RestGuild? guild = await _discordBot.Rest.GetGuildAsync(_discordBot.GuildId).ConfigureAwait(false);
            foreach (Channel channel in data.DiscordChannels.ToArray())
            {
                RestThreadChannel thread =
                    await guild.GetThreadChannelAsync(channel.DiscordId).ConfigureAwait(false);

                if (thread.ArchiveTimestamp - DateTimeOffset.UtcNow < TimeSpan.FromMinutes(45))
                {
                    await thread.ModifyAsync(properties =>
                    {
                        properties.AutoArchiveDuration = ThreadArchiveDuration.OneHour;
                    }).ConfigureAwait(true);
                    await Task.Delay(250).ConfigureAwait(true);
                    await thread.ModifyAsync(properties =>
                    {
                        properties.AutoArchiveDuration = ThreadArchiveDuration.OneDay;
                    }).ConfigureAwait(true);
                    count++;
                }
            }

            if (count > 0) _logger.LogInformation($"Unarchived {count:D} thread(s)");

            _logger.LogInformation("Triggered "
                                   + nameof(ThreadReviver)
                                   + Environment.NewLine
                                   + $"Interval = {Interval.TotalSeconds}s");
        }
        catch (Exception e)
        {
            _logger.LogError(e, null);
        }
    }
}