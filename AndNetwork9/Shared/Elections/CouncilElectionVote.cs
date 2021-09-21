using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Shared.Elections
{
    public record CouncilElectionVote
    {
        public Direction Direction { get; set; }

        public Dictionary<int, uint> Votes { get; set; } = new();
        public bool VoteAllowed { get; set; }
    }
}