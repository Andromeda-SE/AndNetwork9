using System;

namespace AndNetwork9.Shared.Backend.Discord.Enums;

[Flags]
public enum ChannelFlags
{
    None = 0,
    Elections = 0x1,
    Advertisement = 0x2,
    PublicAdvertisement = 0x4,
    Immutable = 0x8,
    FrozenThread = 0x10,
}