using System.Net;
using System.Text.Json.Serialization;
using And9.Integration.Steam.Senders.Converters;
using And9.Lib.Formatters.Json;
using And9.Lib.Formatters.MessagePack;
using MessagePack;

namespace And9.Integration.Steam.Senders.Models;

[MessagePackObject]
public record struct PlayerActivityResultNode
{
    private const string _SPACE_ENGINEERS_APP_ID = "244850";
    [JsonPropertyName("steamid")]
    [JsonConverter(typeof(UInt64FromStringConverter))]
    [Key(0)]
    public ulong SteamId { get; set; }
    [JsonPropertyName("personaname")]
    [Key(1)]
    public string? Nickname { get; set; }
    [JsonPropertyName("realname")]
    [Key(2)]
    public string? RealName { get; set; }
    [JsonPropertyName("gameserverip")]
    [JsonConverter(typeof(IpEndPointConverter))]
    [MessagePackFormatter(typeof(IpEndPointFormatter))]
    [Key(3)]
    public IPEndPoint? GameServerId { get; set; }
    [JsonPropertyName("gameserversteamid")]
    [JsonConverter(typeof(NullableUInt64FromStringConverter))]
    [Key(4)]
    public ulong? GameServerSteamId { get; set; }
    [JsonPropertyName("lobbysteamid")]
    [JsonConverter(typeof(NullableUInt64FromStringConverter))]
    [Key(5)]
    public ulong? LobbySteamId { get; set; }
    [JsonPropertyName("gameid")]
    [Key(6)]
    public string? GameId { get; set; }
    [JsonIgnore]
    [IgnoreMember]
    public bool InSpaceEngineers => GameId == _SPACE_ENGINEERS_APP_ID;
}