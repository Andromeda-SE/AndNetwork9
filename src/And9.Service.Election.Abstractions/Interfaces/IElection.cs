using And9.Lib.Models.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Election.Abstractions.Enums;

namespace And9.Service.Election.Abstractions.Interfaces;

public interface IElection : IId
{
    short ElectionId { get; }
    int IId.Id => ElectionId << 16 + (int)Direction;

    DateOnly AdvisorsStartDate { get; }
    Direction Direction { get; }
    ElectionStatus Status { get; }
    int AgainstAllVotes { get; }
}