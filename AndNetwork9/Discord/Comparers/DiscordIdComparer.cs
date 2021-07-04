using System.Collections.Generic;
using Discord;

namespace AndNetwork9.Discord.Comparers
{
    public class DiscordIdComparer : IEqualityComparer<IEntity<ulong>>, IEqualityComparer<IUser>
    {
        public static readonly DiscordIdComparer Static = new();

        public bool Equals(IEntity<ulong>? x, IEntity<ulong>? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(IEntity<ulong> obj)
        {
            return obj.Id.GetHashCode();
        }

        public bool Equals(IUser? x, IUser? y)
        {
            return Equals(x, (IEntity<ulong>?)y);
        }

        public int GetHashCode(IUser obj)
        {
            return GetHashCode((IEntity<ulong>)obj);
        }
    }
}