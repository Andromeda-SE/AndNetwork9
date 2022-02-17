using And9.Lib.Models.Abstractions.Interfaces;

namespace And9.Service.Squad.Abstractions.Interfaces;

public interface ISquadPart : IId
{
    public short SquadNumber { get; }
    public short SquadPartNumber { get; }
    int IId.Id => (SquadNumber << 16) + SquadPartNumber;
}