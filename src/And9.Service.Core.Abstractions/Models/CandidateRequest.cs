using System.ComponentModel.DataAnnotations;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Properties;
using MessagePack;

namespace And9.Service.Core.Abstractions.Models;

[MessagePackObject]
public record CandidateRequest : ICandidateRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(Resources.CandidateRequest_Nickname_Required))]
    [MaxLength(24, ErrorMessageResourceName = nameof(Resources.CandidateRequest_Nickname_MaxLength))]
    [MessagePack.Key(0)]
    public string Nickname { get; set; } = string.Empty;
    [MessagePack.Key(1)]
    public string? RealName { get; set; }
    [Required(ErrorMessageResourceName = nameof(Resources.CandidateRequest_Steam_Required))]
    [MessagePack.Key(2)]
    public ulong SteamId { get; set; }
    [Required(ErrorMessageResourceName = nameof(Resources.CandidateRequest_Discord_Required))]
    [MessagePack.Key(3)]
    public ulong DiscordId { get; set; }
    [MessagePack.Key(4)]
    public long? VkId { get; set; }
    [Range(0, byte.MaxValue)]
    [MessagePack.Key(5)]
    public int? Age { get; set; }
    [Range(0, int.MaxValue, ErrorMessageResourceName = nameof(Resources.CandidateRequest_HoursCount_Range))]
    [MessagePack.Key(6)]
    public int? HoursCount { get; set; }
    [MessagePack.Key(7)]
    public TimeZoneInfo? TimeZone { get; set; }
    [MaxLength(1024)]
    [MessagePack.Key(8)]
    public string? Recommendation { get; set; }
    [MaxLength(ushort.MaxValue)]
    [MessagePack.Key(9)]
    public string? Description { get; set; }
    [MessagePack.Key(10)]
    public short? AuxiliarySquad { get; set; }
}