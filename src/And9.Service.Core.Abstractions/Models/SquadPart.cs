using And9.Service.Core.Abstractions.Interfaces;
using MessagePack;

namespace And9.Service.Core.Abstractions.Models;

[MessagePackObject]
public class SquadPart : ISquadPart
{
    [Key(0)]
    public short SquadNumber { get; set; }
    [Key(1)]
    public short SquadPartNumber { get; set; }
    [Key(2)]
    public DateTime LastChanged { get; set; }
    [Key(3)]
    public Guid ConcurrencyToken { get; set; }
    [IgnoreMember]
    public IList<Member> Members { get; set; } = new List<Member>();
    [IgnoreMember]
    public Member? Commander => Members.FirstOrDefault(x => x.IsSquadCommander);
}