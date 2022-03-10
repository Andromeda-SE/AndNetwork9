using And9.Lib.Models.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Enums;

namespace And9.Service.Election.Abstractions.Interfaces;

public interface IElectionVote : IId, IComparable<IElectionVote?>, IComparable
{
    short ElectionId { get; }
    Direction Direction { get; }
    int MemberId { get; }
    bool? Voted { get; }
    int Votes { get; }

    int IComparable.CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is IElectionVote other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(IElectionVote)}");
    }

    int IComparable<IElectionVote?>.CompareTo(IElectionVote? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        int votesComparison = Votes.CompareTo(other.Votes);
        if (votesComparison != 0) return votesComparison;
        return MemberId.CompareTo(other.MemberId);
    }
}