using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Models;

namespace And9.Service.Core.API.Interfaces;

public interface ICoreServerMethods
{
    Task<Member?> ReadMe();

    Task RegisterCandidate(CandidateRequest request);
    Task AcceptCandidate(int id);
    Task DeclineCandidate(int id);

    Task ChangeNickname(string newNickname);
    Task ChangeRealName(string newRealName);
    Task ChangeTimezone(string timezoneId);

    Task Kick(int memberId);
    Task Exile(int memberId);

    Task CreateSquad(short? number);
    Task DisbandSquad();
    Task<ISquad> ReadSquad(short number);
    IAsyncEnumerable<ISquad> ReadAllSquads();

    Task AppendSquadName(short number, string name);

    Task CreateSquadPart(int leaderId);
    Task MoveMemberToSquadPart(short targetSquadPart, int memberId);
    Task SetSquadPartLeader(int memberId);
    Task SetSquadPartLeader(short targetSquadPart, int memberId);

    Task SendSquadJoinRequest(short squadNumber);
    Task AcceptSquadJoinRequest(short squadNumber, short squadPartNumber, int memberId);
    Task DeclineSquadJoinRequest(short squadNumber, int memberId);
    Task CancelSquadJoinRequest(short squadNumber);
    IAsyncEnumerable<ISquadRequest> ReadSquadJoinRequests(short squadNumber);
    IAsyncEnumerable<ISquadRequest> ReadMySquadJoinRequests();

    Task RiseAuxiliary(int memberId);
    Task DemoteAuxiliary(int memberId);

    Task KickFromSquad(int memberId);
    Task LeaveFromSquad();

    IAsyncEnumerable<ISquadMembershipHistoryEntry> ReadSquadMembershipHistory(short squadNumber);
    IAsyncEnumerable<ISquadMembershipHistoryEntry> ReadMemberSquadMembershipHistory(int memberId);
}