using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Enums;

namespace And9.Integration.Discord.Database.Models;

public record class Channel : IChannel
{
    public ChannelCategory? Category { get; set; }
    public Direction? Direction { get; set; }
    public ulong DiscordId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DiscordChannelType Type { get; set; }
    public DiscordChannelFlags DiscordChannelFlags { get; set; }
    public int? CategoryId { get; set; }
    public int ChannelPosition { get; set; }
    public short? SquadNumber { get; set; }
    public short? SquadPartNumber { get; set; }
    public DiscordPermissions EveryonePermissions { get; set; }
    public DiscordPermissions MemberPermissions { get; set; }
    public DiscordPermissions SquadPartPermissions { get; set; }
    public DiscordPermissions SquadPermissions { get; set; }
    public DiscordPermissions SquadLieutenantsPermissions { get; set; }
    public DiscordPermissions SquadCaptainPermissions { get; set; }
    public DiscordPermissions AdvisorPermissions { get; set; }
    public DateTime LastChanged { get; set; }
    public Guid ConcurrencyToken { get; set; }
}