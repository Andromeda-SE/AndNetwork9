using System.Text.Json.Serialization;
using And9.Integration.Steam.Senders.Converters;

namespace And9.Integration.Steam.Models;

public record ResolveSteamUrlResult
{
    [JsonPropertyName("steamid")]
    [JsonConverter(typeof(NullableUInt64FromStringConverter))]
    public ulong? SteamId { get; set; }
}