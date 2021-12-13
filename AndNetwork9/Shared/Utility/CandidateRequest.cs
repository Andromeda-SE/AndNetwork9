using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Converters;

namespace AndNetwork9.Shared.Utility;

public record CandidateRequest
{
    [Required(ErrorMessage = "Никнейм не может быть пустым")]
    [StringLength(24, MinimumLength = 1, ErrorMessage = "В поле «Никнейм» должно быть от 1 до 24 символов")]
    public string Nickname { get; set; }

    public string? RealName { get; set; }
    [Required]
    public ulong SteamId { get; set; }
    [Required]
    public ulong DiscordId { get; set; }

    public long? VkId { get; set; }
    [Range(0, 256, ErrorMessage = "Поле «Возраст» должно быть в диапазоне от 0 до 256")]
    public int? Age { get; set; }
    [Range(0, 100000, ErrorMessage = "Поле «Часов в игре» должно быть в диапазоне от 0 до 100000")]
    public int? HoursCount { get; set; }

    [JsonConverter(typeof(TimeZoneInfoConverter))]
    public TimeZoneInfo? TimeZone { get; set; }
    public string? Recommendation { get; set; }

    public string? Description { get; set; }

    public string GetDiscordMention() => $"<@{DiscordId:D}>";

    public string GetSteamLink() => $"http://steamcommunity.com/profiles/{SteamId:D}";

    public string? GetVkLink() => VkId.HasValue ? $"http://vk.com/id{VkId:D}" : null;
}