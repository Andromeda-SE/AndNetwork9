using System;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Shared.Backend.Senders.Elections
{
    public record VoteArg(int MemberId, Guid Key, Direction Direction, VoteArgNode[] Votes);
}