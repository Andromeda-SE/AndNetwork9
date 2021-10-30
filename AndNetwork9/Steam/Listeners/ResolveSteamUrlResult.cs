using System.Text.Json.Serialization;
using AndNetwork9.Shared.Converters;

namespace AndNetwork9.Steam.Listeners;

public record ResolveSteamUrlResult
{
    [JsonPropertyName("steamid")]
    [JsonConverter(typeof(NullableUInt64FromStringConverter))]
    public ulong? SteamId { get; set; }
}