using System;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Shared.Votings
{
    public record Vote
    {
        public int VotingId { get; set; }
        public virtual Voting Voting { get; set; } = null!;
        public int MemberId { get; set; }
        public virtual Member Member { get; set; } = null!;
        public DateTime? VoteTime { get; set; }
        public MemberVote Result { get; set; }
    }
}