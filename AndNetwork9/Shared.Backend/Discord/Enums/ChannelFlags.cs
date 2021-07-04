using System;

namespace AndNetwork9.Shared.Backend.Discord.Enums
{
    [Flags]
    public enum ChannelFlags
    {
        None = 0,
        Elections = 1,
        Advertisement = 2,
        PublicAdvertisement = 4,
        Immutable = 8,
    }
}