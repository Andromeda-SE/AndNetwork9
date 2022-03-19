using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Enums;
using MessagePack;

namespace And9.Integration.Discord.Abstractions.Models;

[MessagePackObject]
public record class Channel : IChannel
{
    [IgnoreMember]
    public ChannelCategory? Category { get; set; }
    [Key(0)]
    public ulong DiscordId { get; set; }
    [Key(1)]
    public string Name { get; set; } = string.Empty;
    [Key(2)]
    public Direction? Direction { get; set; }
    [Key(3)]
    public DiscordChannelType Type { get; set; }
    [Key(4)]
    public DiscordChannelFlags DiscordChannelFlags { get; set; }
    [Key(5)]
    public int? CategoryId { get; set; }
    [Key(6)]
    public int ChannelPosition { get; set; }
    [Key(7)]
    public short? SquadNumber { get; set; }
    [Key(8)]
    public short? SquadPartNumber { get; set; }
    [Key(9)]
    public DiscordPermissions EveryonePermissions { get; set; }
    [Key(10)]
    public DiscordPermissions MemberPermissions { get; set; }
    [Key(11)]
    public DiscordPermissions SquadPartPermissions { get; set; }
    [Key(12)]
    public DiscordPermissions SquadPartCommanderPermissions { get; set; }
    [Key(13)]
    public DiscordPermissions SquadPermissions { get; set; }
    [Key(14)]
    public DiscordPermissions SquadLieutenantsPermissions { get; set; }
    [Key(15)]
    public DiscordPermissions SquadCaptainPermissions { get; set; }
    [Key(16)]
    public DiscordPermissions AdvisorPermissions { get; set; }
    [Key(17)]
    public DateTime LastChanged { get; set; }
    [Key(18)]
    public Guid ConcurrencyToken { get; set; }
}