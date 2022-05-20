namespace And9.Service.Core.API.Interfaces;

public interface ICoreClientMethods
{
    Task SquadRequestDeclined(short squadNumber);

    Task NewSquadRequest(int memberId);
}