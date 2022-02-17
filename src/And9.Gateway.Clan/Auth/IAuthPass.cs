using And9.Service.Core.Abstractions.Interfaces;

namespace And9.Gateway.Clan.Auth;

public interface IAuthPass
{
    bool Pass(IMember member);
}