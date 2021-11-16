using System;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Interfaces;

namespace AndNetwork9.Shared.Votings;

public record Vote : IConcurrencyToken
{
    public int VotingId { get; set; }
    public virtual Voting Voting { get; set; } = null!;
    public int MemberId { get; set; }
    public virtual Member Member { get; set; } = null!;
    public DateTime? VoteTime { get; set; }
    public MemberVote Result { get; set; }
    public Guid ConcurrencyToken { get; set; }
    public DateTime LastChanged { get; set; }
}