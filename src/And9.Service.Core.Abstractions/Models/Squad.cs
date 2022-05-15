using And9.Service.Core.Abstractions.Interfaces;
using MessagePack;

namespace And9.Service.Core.Abstractions.Models;

[MessagePackObject]
public class Squad : ISquad
{
    [Key(0)]
    public short Number { get; set; }
    [Key(1)]
    public List<string> Names { get; set; } = new ();
    [Key(2)]
    public DateOnly CreateDate { get; set; }
    [Key(3)]
    public bool IsActive { get; set; }
    [IgnoreMember]
    public IList<SquadMembershipHistoryEntry> SquadMembershipHistoryEntries { get; set; } = new List<SquadMembershipHistoryEntry>();
    [IgnoreMember]
    public IList<Member> Members { get; set; } = new List<Member>();
    [IgnoreMember]
    public IList<SquadRequest> SquadRequests { get; set; } = new List<SquadRequest>();
    [Key(4)]
    public DateTime LastChanged { get; set; }
    [Key(5)]
    public Guid ConcurrencyToken { get; set; }
}