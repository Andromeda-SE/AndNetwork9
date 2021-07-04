using System;
using System.Collections.Generic;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Shared.Elections
{
    public record CouncilElectionVote
    {
        public Guid Key { get; set; }
        public Direction Direction { get; set; }

        public Dictionary<int, uint> Votes { get; set; } = new();
    }
}