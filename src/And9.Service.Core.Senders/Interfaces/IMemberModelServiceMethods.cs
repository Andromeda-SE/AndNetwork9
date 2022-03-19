using And9.Service.Core.Abstractions.Models;

namespace And9.Service.Core.Senders.Interfaces;

public interface IMemberModelServiceMethods
{
    Task<Member?> ReadBySteamId(ulong steamId);
    Task<Member?> ReadByDiscordId(ulong discordId);
    Task<Member?> ReadByNickname(string nickname);
}