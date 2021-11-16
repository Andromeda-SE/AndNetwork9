using System;
using System.Collections.Generic;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Interfaces;

namespace AndNetwork9.Shared.Elections;

public record CouncilElectionVote : IConcurrencyToken
{
    public Direction Direction { get; set; }

    public Dictionary<int, uint> Votes { get; set; } = new();
    public bool VoteAllowed { get; set; }
    public Guid ConcurrencyToken { get; set; }
    public DateTime LastChanged { get; set; }
}