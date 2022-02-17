using Microsoft.AspNetCore.SignalR.Client;

namespace And9.Gateway.Clan.Client.Interfaces;

public interface IClientConnection
{
    HubConnection Connection { get; }

    async Task TokenProviderOnTokenChanged()
    {
        await Connection.StopAsync().ConfigureAwait(false);
        await Connection.StartAsync().ConfigureAwait(false);
    }
}