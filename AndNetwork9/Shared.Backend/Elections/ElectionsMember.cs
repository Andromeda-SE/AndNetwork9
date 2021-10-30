using System;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Interfaces;

namespace AndNetwork9.Shared.Backend.Elections;

public record ElectionsMember : IComparable<ElectionsMember>, IConcurrencyToken
{
    public int ElectionId { get; set; }
    public Direction Direction { get; set; }
    public int MemberId { get; set; }

    [JsonIgnore]
    public virtual ElectionVoting Voting { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;
    public int? Votes { get; set; }
    public DateTime? VotedTime { get; set; }
    public bool Voted { get; set; }

    public int CompareTo(ElectionsMember? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        int votesComparison = Nullable.Compare(Votes, other.Votes);
        if (votesComparison != 0) return votesComparison;
        return Nullable.Compare(VotedTime, other.VotedTime);
    }

    public Guid ConcurrencyToken { get; set; }
}