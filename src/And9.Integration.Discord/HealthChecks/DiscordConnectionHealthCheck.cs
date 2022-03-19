using Discord;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace And9.Integration.Discord.HealthChecks;

public class DiscordConnectionHealthCheck : IHealthCheck
{
    private readonly DiscordBot _bot;
    public DiscordConnectionHealthCheck(DiscordBot bot) => _bot = bot;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_bot.ConnectionState switch
        {
            ConnectionState.Connecting => HealthCheckResult.Degraded(),
            ConnectionState.Connected when _bot.LoginState == LoginState.LoggingIn => HealthCheckResult.Degraded(),
            ConnectionState.Connected when _bot.LoginState == LoginState.LoggedIn => HealthCheckResult.Healthy(),
            _ => HealthCheckResult.Unhealthy(),
        });
    }
}