using System;
using System.Collections.Generic;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Shared.Elections
{
    public record CouncilElectionTokenPack
    {
        public int ElectionId { get; set; }
        public int MemberId { get; set; }

        public Dictionary<Direction, Guid> Tokens { get; set; } = new();
    }
}