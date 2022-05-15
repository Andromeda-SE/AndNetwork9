namespace And9.Service.Core.API.Interfaces;

public interface ICoreClientMethods
{
    Task NewMemberInSquad(int memberId);
    Task LeaveMemberInSquad(int memberId);
    Task NewMySquadLeader(int memberId);
    Task NewMySquadPartLeader(int memberId);
    Task NewMySquadPart(short newSquadPart);
    Task MeKickedFromSquad();

    Task SquadRequestDeclined(short squadNumber);
    Task NewMySquad(short squadNumber);
}