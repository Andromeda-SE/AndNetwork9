using System;
using System.Collections.Generic;

namespace AndNetwork9.Shared.Backend.Discord.Channels
{
    public class Category
    {
        public ulong DiscordId { get; set; }
        public int Position { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual IList<Channel> Channels { get; set; } = Array.Empty<Channel>();
    }
}