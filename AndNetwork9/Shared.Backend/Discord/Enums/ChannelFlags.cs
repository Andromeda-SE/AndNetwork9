using System;

namespace AndNetwork9.Shared.Backend.Discord.Enums;

[Flags]
public enum ChannelFlags
{
    None = 0,
    Elections = 0b1,
    Advertisement = 0b10,
    PublicAdvertisement = 0b100,
    Immutable = 0b1000,
    FrozenThread = 0b10000,
    CandidatesChannel = 0b100000,
    BotLog = 0b1000000,
}