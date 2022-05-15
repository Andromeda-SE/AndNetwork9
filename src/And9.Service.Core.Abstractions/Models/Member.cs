using System.Text.Json.Serialization;
using And9.Lib.Formatters.Json;
using And9.Lib.Formatters.MessagePack;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Interfaces;
using MessagePack;

namespace And9.Service.Core.Abstractions.Models;

[MessagePackObject]
public record Member : IMember
{
    [IgnoreMember]
    [JsonIgnore]
    public string RankIcon => $"{this.GetSquadMemberLevel().GetIconString()}{Rank.GetIconString()}";
    [Key(0)]

    public int Id { get; set; }
    [Key(1)]
    public ulong? DiscordId { get; set; }
    [Key(2)]
    public ulong? SteamId { get; set; }
    [Key(3)]
    public long? MicrosoftId { get; set; }
    [Key(4)]
    public long? VkId { get; set; }
    [Key(5)]
    public long? TelegramId { get; set; }
    [Key(6)]
    public string Nickname { get; set; } = string.Empty;
    [Key(7)]
    public string? RealName { get; set; }
    [Key(8)]
    public Rank Rank { get; set; }
    [Key(9)]
    public IList<MemberSpecialization> Specializations { get; set; } = new List<MemberSpecialization>();
    [Key(10)]
    public bool IsSquadCommander { get; set; }
    [Key(11)]
    public short? SquadNumber { get; set; }
    [IgnoreMember]
    public Squad? Squad { get; set; }
    [Key(12)]
    public short SquadPartNumber { get; set; }
    [IgnoreMember]
    public SquadPart? SquadPart { get; set; }
    [Key(13)]
    [MessagePackFormatter(typeof(TimeZoneInfoFormatter))]
    [JsonConverter(typeof(TimeZoneInfoConverter))]
    public TimeZoneInfo? TimeZone { get; set; }
    [Key(14)]
    [MessagePackFormatter(typeof(DateOnlyFormatter))]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly JoinDate { get; set; }
    [IgnoreMember]
    public IList<SquadMembershipHistoryEntry> SquadMembershipHistoryEntries { get; set; } = new List<SquadMembershipHistoryEntry>();
    [IgnoreMember]
    public IList<SquadRequest> SquadRequests { get; set; } = new List<SquadRequest>();
    [Key(16)]
    public DateTime LastChanged { get; set; }
    [Key(17)]
    public Guid ConcurrencyToken { get; set; }

    public override string ToString()
    {
        string? rankIcon = Rank.GetIconString();
        string result = string.Empty;
        if (rankIcon is not null) result += $"[{rankIcon}] ";
        result += Nickname;
        if (RealName is not null && result.Length + RealName.Length + 3 <= 32) result += $" ({RealName})";
        return result;
    }
}