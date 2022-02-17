using And9.Gateway.Clan.Client.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace And9.Gateway.Clan.Client.Connections;

public abstract class ConnectionBase : IClientConnection, IAsyncDisposable, IHostedService
{
    private readonly ILogger<ConnectionBase> _logger;
    private readonly IAuthTokenProvider _tokenProvider;
    private bool _started;

    protected ConnectionBase(HubConnection connection, IAuthTokenProvider tokenProvider, ILogger<ConnectionBase> logger)
    {
        _tokenProvider = tokenProvider;
        _logger = logger;
        Connection = connection;
        _tokenProvider.TokenChanged += TokenProviderOnTokenChanged;
    }

    public ValueTask DisposeAsync()
    {
        _tokenProvider.TokenChanged -= TokenProviderOnTokenChanged;
        return ValueTask.CompletedTask;
    }

    public HubConnection Connection { get; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (Connection.State != HubConnectionState.Disconnected) return;
        _logger.LogInformation($"{GetType().FullName}: Starting");
        await Connection.StartAsync(cancellationToken).ConfigureAwait(false);
        _started = true;
        _logger.LogInformation($"{GetType().FullName}: Started");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{GetType().FullName}: Stopping");
        await Connection.StopAsync(cancellationToken).ConfigureAwait(false);
        _started = false;
        _logger.LogInformation($"{GetType().FullName}: Stopped");
    }

    private async Task TokenProviderOnTokenChanged(string obj)
    {
        if (_started && Connection.State == HubConnectionState.Connected)
        {
            await StopAsync(CancellationToken.None).ConfigureAwait(false);
            await StartAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}