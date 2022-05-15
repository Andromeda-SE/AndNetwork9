using And9.Lib.Models.Abstractions.Interfaces;

namespace And9.Service.Core.Abstractions.Interfaces;

public interface ISquadMembershipHistoryEntry : IId
{
    public int MemberId { get; }
    public short SquadId { get; }
    public DateTime JoinDateTime { get; }
    public DateTime? LeaveDateTime { get; }
}