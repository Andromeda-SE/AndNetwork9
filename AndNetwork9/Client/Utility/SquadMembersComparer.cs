using System.Collections.Generic;
using AndNetwork9.Shared;

namespace AndNetwork9.Client.Utility;

public class SquadMembersComparer : IComparer<Member>
{
    public static SquadMembersComparer Shared { get; } = new();

    public int Compare(Member x, Member y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;
        int commanderLevelComparison = x.CommanderLevel.CompareTo(y.CommanderLevel);
        if (commanderLevelComparison != 0) return commanderLevelComparison;
        return x.CompareTo(y);
    }
}