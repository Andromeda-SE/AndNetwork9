using And9.Gateway.Clan.Client.Interfaces;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.API.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace And9.Gateway.Clan.Client.Connections;

public class CoreConnection : ConnectionBase, ICoreServerMethods
{
    public CoreConnection(HubConnection connection, IAuthTokenProvider tokenProvider, ILogger<CoreConnection> logger) : base(connection, tokenProvider, logger) { }
    public async Task<Member?> ReadMe() => await Connection.InvokeAsync<Member?>(nameof(ReadMe)).ConfigureAwait(false);

    public async Task RegisterCandidate(CandidateRequest request) => await Connection.InvokeAsync<string?>(nameof(RegisterCandidate), request).ConfigureAwait(false);

    public async Task AcceptCandidate(int id) => await Connection.InvokeAsync<string?>(nameof(AcceptCandidate), id).ConfigureAwait(false);

    public async Task DeclineCandidate(int id) => await Connection.InvokeAsync<string?>(nameof(AcceptCandidate), id).ConfigureAwait(false);

    public async Task ChangeNickname(string newNickname) => await Connection.InvokeAsync<string?>(nameof(ChangeNickname), newNickname).ConfigureAwait(false);

    public async Task ChangeRealName(string newRealName) => await Connection.InvokeAsync<string?>(nameof(ChangeRealName), newRealName).ConfigureAwait(false);

    public async Task ChangeDirection(Direction direction) => await Connection.InvokeAsync<string?>(nameof(ChangeDirection), direction).ConfigureAwait(false);

    public async Task ChangeTimezone(string timezoneId) => await Connection.InvokeAsync<string?>(nameof(ChangeTimezone), timezoneId).ConfigureAwait(false);

    public async Task Kick(int memberId) => await Connection.InvokeAsync<string?>(nameof(Kick), memberId).ConfigureAwait(false);

    public async Task Exile(int memberId) => await Connection.InvokeAsync<string?>(nameof(Exile), memberId).ConfigureAwait(false);
}