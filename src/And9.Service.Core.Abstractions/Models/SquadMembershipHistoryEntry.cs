using And9.Service.Core.Abstractions.Interfaces;
using MessagePack;

namespace And9.Service.Core.Abstractions.Models;

[MessagePackObject]
public class SquadMembershipHistoryEntry : ISquadMembershipHistoryEntry
{
    [IgnoreMember]
    public Member Member { get; set; }
    [IgnoreMember]
    public Squad Squad { get; set; }
    [Key(0)]
    public int Id { get; set; }
    [Key(1)]
    public int MemberId { get; set; }
    [Key(2)]
    public short SquadId { get; set; }
    [Key(3)]
    public DateTime JoinDateTime { get; set; }
    [Key(4)]
    public DateTime? LeaveDateTime { get; set; }
    [Key(5)]
    public DateTime LastChanged { get; set; }
    [Key(6)]
    public Guid ConcurrencyToken { get; set; }
}