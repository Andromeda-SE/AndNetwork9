using And9.Service.Core.Abstractions.Enums;
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
    Task ChangeDirection(Direction direction);
    Task ChangeTimezone(string timezoneId);

    Task Kick(int memberId);
    Task Exile(int memberId);
}