using System;
using System.Collections.Generic;
using AndNetwork9.Shared.Interfaces;

namespace AndNetwork9.Shared.Backend.Discord.Channels;

public class Category : IConcurrencyToken
{
    public ulong DiscordId { get; set; }
    public int Position { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual IList<Channel> Channels { get; set; } = Array.Empty<Channel>();
    public Guid ConcurrencyToken { get; set; }
    public DateTime LastChanged { get; set; }
}