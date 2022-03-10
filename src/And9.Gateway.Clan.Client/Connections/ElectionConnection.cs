using And9.Gateway.Clan.Client.Interfaces;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Election.Abstractions.Models;
using And9.Service.Election.API.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace And9.Gateway.Clan.Client.Connections;

public class ElectionConnection : ConnectionBase, IElectionServerMethods
{
    public ElectionConnection(HubConnection connection, IAuthTokenProvider tokenProvider, ILogger<ConnectionBase> logger) : base(connection, tokenProvider, logger) { }
    public async Task<bool> Register() => await Connection.InvokeAsync<bool>(nameof(IElectionServerMethods.Register)).ConfigureAwait(false);

    public async Task<bool> CancelRegister() => await Connection.InvokeAsync<bool>(nameof(IElectionServerMethods.CancelRegister)).ConfigureAwait(false);

    public async Task<bool> Vote(IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>> votes)
        => await Connection.InvokeAsync<bool>(nameof(IElectionServerMethods.Vote), votes).ConfigureAwait(false);

    public IAsyncEnumerable<Election> GetElection(CancellationToken token)
        => Connection.StreamAsync<Election>(nameof(IElectionServerMethods.GetElection), token);
}