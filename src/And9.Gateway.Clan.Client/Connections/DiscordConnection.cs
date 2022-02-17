using And9.Gateway.Clan.Client.Interfaces;
using And9.Integration.Discord.API.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace And9.Gateway.Clan.Client.Connections;

public class DiscordConnection : ConnectionBase, IDiscordServerMethods
{
    public DiscordConnection(HubConnection connection, IAuthTokenProvider tokenProvider, ILogger<DiscordConnection> logger) : base(connection, tokenProvider, logger) { }
    public async Task SendDirectMessageAsync(ulong discordId, string message) => await Connection.InvokeAsync(nameof(SendDirectMessageAsync), discordId, message).ConfigureAwait(false);
}