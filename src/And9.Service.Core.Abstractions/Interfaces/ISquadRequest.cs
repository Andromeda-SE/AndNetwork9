using And9.Lib.Models.Abstractions.Interfaces;

namespace And9.Service.Core.Abstractions.Interfaces;

public interface ISquadRequest : IId
{
    public int MemberId { get; }
    public short SquadNumber { get; }
    public bool? Accepted { get; }
    public bool IsCanceledByMember { get; }
    public DateTime CreateDateTime { get; }
}