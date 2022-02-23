using And9.Lib.Models.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Enums;

namespace And9.Service.Election.Abstractions.Interfaces;

public interface IElectionVote : IId
{
    short ElectionId { get; }
    Direction Direction { get; }
    int? MemberId { get; }
    bool? Voted { get; }
    int Votes { get; }
}