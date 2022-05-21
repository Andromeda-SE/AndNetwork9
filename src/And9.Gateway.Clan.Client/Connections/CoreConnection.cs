using And9.Gateway.Clan.Client.Interfaces;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.API.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace And9.Gateway.Clan.Client.Connections;

public class CoreConnection : ConnectionBase, ICoreServerMethods
{
    public CoreConnection(HubConnection connection, IAuthTokenProvider tokenProvider, ILogger<CoreConnection> logger) : base(connection, tokenProvider, logger) { }
    public async Task<Member?> ReadMe() => await Connection.InvokeAsync<Member?>(nameof(ReadMe)).ConfigureAwait(false);

    public async Task RegisterCandidate(CandidateRequest request) => await Connection.InvokeAsync(nameof(RegisterCandidate), request).ConfigureAwait(false);

    public async Task AcceptCandidate(int id) => await Connection.InvokeAsync(nameof(AcceptCandidate), id).ConfigureAwait(false);

    public async Task DeclineCandidate(int id) => await Connection.InvokeAsync(nameof(AcceptCandidate), id).ConfigureAwait(false);

    public async Task ChangeNickname(string newNickname) => await Connection.InvokeAsync(nameof(ChangeNickname), newNickname).ConfigureAwait(false);

    public async Task ChangeRealName(string newRealName) => await Connection.InvokeAsync(nameof(ChangeRealName), newRealName).ConfigureAwait(false);

    public async Task ChangeTimezone(string timezoneId) => await Connection.InvokeAsync(nameof(ChangeTimezone), timezoneId).ConfigureAwait(false);

    public async Task Kick(int memberId) => await Connection.InvokeAsync(nameof(Kick), memberId).ConfigureAwait(false);

    public async Task Exile(int memberId) => await Connection.InvokeAsync(nameof(Exile), memberId).ConfigureAwait(false);
    public async Task CreateSquad(short? number) => await Connection.InvokeAsync(nameof(CreateSquad), number).ConfigureAwait(false);

    public async Task DisbandSquad() => await Connection.InvokeAsync(nameof(DisbandSquad)).ConfigureAwait(false);

    public async Task<ISquad> ReadSquad(short number) => await Connection.InvokeAsync<ISquad>(nameof(Exile), number).ConfigureAwait(false);

    public IAsyncEnumerable<ISquad> ReadAllSquads() => Connection.StreamAsync<ISquad>(nameof(ReadAllSquads));

    public async Task AppendSquadName(short number, string name) => await Connection.InvokeAsync(nameof(AppendSquadName), number, name).ConfigureAwait(false);

    public async Task CreateSquadPart(int leaderId) => await Connection.InvokeAsync(nameof(CreateSquadPart), leaderId).ConfigureAwait(false);

    public async Task MoveMemberToSquadPart(short targetSquadPart, int memberId) => await Connection.InvokeAsync(nameof(MoveMemberToSquadPart), targetSquadPart, memberId).ConfigureAwait(false);

    public async Task SetMySquadPartLeader(int memberId) => await Connection.InvokeAsync(nameof(SetMySquadPartLeader), memberId).ConfigureAwait(false);
    public async Task SetSquadPartLeader(short targetSquadPart, int memberId) => await Connection.InvokeAsync(nameof(SetMySquadPartLeader), targetSquadPart, memberId).ConfigureAwait(false);

    public async Task SendSquadJoinRequest(short squadNumber) => await Connection.InvokeAsync(nameof(SendSquadJoinRequest), squadNumber).ConfigureAwait(false);

    public async Task AcceptSquadJoinRequest(short squadNumber, short squadPartNumber, int memberId) => await Connection.InvokeAsync(nameof(AcceptSquadJoinRequest), squadNumber, squadPartNumber, memberId).ConfigureAwait(false);

    public async Task DeclineSquadJoinRequest(short squadNumber, int memberId) => await Connection.InvokeAsync(nameof(DeclineSquadJoinRequest), squadNumber, memberId).ConfigureAwait(false);

    public async Task CancelSquadJoinRequest(short squadNumber) => await Connection.InvokeAsync(nameof(CancelSquadJoinRequest), squadNumber).ConfigureAwait(false);

    public IAsyncEnumerable<ISquadRequest> ReadSquadJoinRequests(short squadNumber) => Connection.StreamAsync<ISquadRequest>(nameof(ReadSquadJoinRequests), squadNumber);
    public IAsyncEnumerable<ISquadRequest> ReadMySquadJoinRequests() => Connection.StreamAsync<ISquadRequest>(nameof(ReadMySquadJoinRequests));

    public async Task RiseAuxiliary(int memberId) => await Connection.InvokeAsync(nameof(RiseAuxiliary), memberId).ConfigureAwait(false);

    public async Task DemoteAuxiliary(int memberId) => await Connection.InvokeAsync(nameof(DemoteAuxiliary), memberId).ConfigureAwait(false);

    public async Task KickFromSquad(int memberId) => await Connection.InvokeAsync(nameof(KickFromSquad), memberId).ConfigureAwait(false);
    public async Task LeaveFromSquad() => await Connection.InvokeAsync(nameof(LeaveFromSquad)).ConfigureAwait(false);

    public IAsyncEnumerable<ISquadMembershipHistoryEntry> ReadSquadMembershipHistory(short squadNumber) => Connection.StreamAsync<ISquadMembershipHistoryEntry>(nameof(ReadSquadMembershipHistory), squadNumber);

    public IAsyncEnumerable<ISquadMembershipHistoryEntry> ReadMemberSquadMembershipHistory(int memberId) => Connection.StreamAsync<ISquadMembershipHistoryEntry>(nameof(ReadMemberSquadMembershipHistory), memberId);
    public IAsyncEnumerable<Specialization> ReadAllSpecializations() => Connection.StreamAsync<Specialization>(nameof(ReadAllSpecializations));

    public async Task ApproveSpecialization(int memberId, int specializationId) => await Connection.InvokeAsync(nameof(ApproveSpecialization), memberId, specializationId).ConfigureAwait(false);

    public async Task WithdrawSpecialization(int memberId, int specializationId) => await Connection.InvokeAsync(nameof(WithdrawSpecialization), memberId, specializationId).ConfigureAwait(false);

    public async Task ChangeDirection(Direction direction) => await Connection.InvokeAsync(nameof(ChangeDirection), direction).ConfigureAwait(false);

    public async Task SendSquadJoinRequest(short squadNumber, int memberId) => await Connection.InvokeAsync(nameof(SendSquadJoinRequest), squadNumber, memberId).ConfigureAwait(false);
}