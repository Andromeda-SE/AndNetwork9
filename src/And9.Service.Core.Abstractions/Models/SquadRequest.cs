using And9.Service.Core.Abstractions.Interfaces;
using MessagePack;

namespace And9.Service.Core.Abstractions.Models;

[MessagePackObject]
public record class SquadRequest : ISquadRequest
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
    public short SquadNumber { get; set; }
    [Key(3)]
    public bool? Accepted { get; set; }
    [Key(4)]
    public bool IsCanceledByMember { get; set; }
    [Key(5)]
    public DateTime CreateDateTime { get; set; }
    [Key(6)]
    public DateTime LastChanged { get; set; }
    [Key(7)]
    public Guid ConcurrencyToken { get; set; }
}