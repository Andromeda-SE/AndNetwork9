using System.Collections.Generic;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Shared.Backend.Senders.Elections
{
    public record VoteArg(int MemberId, Dictionary<Direction, Dictionary<int, uint>> Bulletin);
}