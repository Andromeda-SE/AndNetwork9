using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace And9.Gateway.Clan.Auth;

public class MemberIdProvider : IUserIdProvider
{
    public virtual string? GetUserId(HubConnectionContext connection) => connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}