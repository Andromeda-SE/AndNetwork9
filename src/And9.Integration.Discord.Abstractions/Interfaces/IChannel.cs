using And9.Integration.Discord.Abstractions.Enums;
using And9.Lib.Models.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Enums;

namespace And9.Integration.Discord.Abstractions.Interfaces;

public interface IChannel : IConcurrencyToken, IDiscordId
{
    public string Name { get; }
    DiscordChannelType Type { get; }
    DiscordChannelFlags DiscordChannelFlags { get; }
    int? CategoryId { get; }
    int ChannelPosition { get; }
    Direction? Direction { get; }
    short? SquadNumber { get; }
    short? SquadPartNumber { get; }
    DiscordPermissions EveryonePermissions { get; }
    DiscordPermissions MemberPermissions { get; }
    DiscordPermissions SquadPartPermissions { get; }
    DiscordPermissions SquadPartCommanderPermissions { get; }
    DiscordPermissions SquadPermissions { get; }
    DiscordPermissions SquadLieutenantsPermissions { get; }
    DiscordPermissions SquadCaptainPermissions { get; }
    DiscordPermissions AdvisorPermissions { get; }
}