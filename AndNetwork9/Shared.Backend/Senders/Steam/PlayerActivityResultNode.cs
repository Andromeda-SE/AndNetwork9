using System.Net;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Converters;

namespace AndNetwork9.Shared.Backend.Senders.Steam
{
    public record PlayerActivityResultNode
    {
        public const string SPACE_ENGINEERS_APP_ID = "244850";
        [JsonPropertyName("steamid")]
        [JsonConverter(typeof(UInt64FromStringConverter))]
        public ulong SteamId { get; set; }
        [JsonPropertyName("personaname")]
        public string? Nickname { get; set; }
        [JsonPropertyName("realname")]
        public string? RealName { get; set; }
        [JsonPropertyName("gameserverip")]
        [JsonConverter(typeof(IpEndPointConverter))]
        public IPEndPoint? GameServerId { get; set; }
        [JsonPropertyName("gameserversteamid")]
        [JsonConverter(typeof(NullableUInt64FromStringConverter))]
        public ulong? GameServerSteamId { get; set; }
        [JsonPropertyName("gameid")]
        public string? GameId { get; set; }
        [JsonIgnore]
        public bool InSpaceEngineers => GameId == SPACE_ENGINEERS_APP_ID;
    }
}