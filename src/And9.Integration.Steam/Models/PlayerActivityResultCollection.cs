using System.Text.Json.Serialization;
using And9.Integration.Steam.Senders.Models;

namespace And9.Integration.Steam.Models;

public record PlayerActivityResultCollection
{
    [JsonPropertyName("players")]
    public PlayerActivityResultNode[]? Players { get; set; }
}