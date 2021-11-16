using Microsoft.AspNetCore.SignalR;

namespace AndNetwork9.Server.Utility;

public class MemberIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) => connection.User.FindFirst("MemberId")?.Value;
    
}