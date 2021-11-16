using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Converters;

namespace AndNetwork9.Shared.Utility;

public record CandidateRequest
{
    [Required]
    [StringLength(24, MinimumLength = 1)]
    public string Nickname { get; set; }
    [Required]
    [Range(0, 256)]
    public uint Age { get; set; }
    [Required]
    public ulong SteamId { get; set; }
    
    public uint? VkId { get; set; }
    public string? Recommendation { get; set; }
    public uint? HoursCount { get; set; }

    [JsonConverter(typeof(TimeZoneInfoConverter))]
    public TimeZoneInfo? TimeZone { get; set; }

    public string? RealName { get; set; }

    public string? Description { get; set; }
    
}