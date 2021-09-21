using System.Text.Json.Serialization;
using AndNetwork9.Shared.Backend.Senders.Steam;

namespace AndNetwork9.Steam.Listeners
{
    public record PlayerActivityResultCollection
    {
        [JsonPropertyName("players")]
        public PlayerActivityResultNode[] Players { get; set; }
    }
}