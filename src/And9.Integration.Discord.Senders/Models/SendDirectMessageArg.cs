using MessagePack;

namespace And9.Integration.Discord.Senders.Models;

[MessagePackObject]
public record struct SendDirectMessageArg([property: Key(0)] ulong DiscordId, [property: Key(1)] string Message);