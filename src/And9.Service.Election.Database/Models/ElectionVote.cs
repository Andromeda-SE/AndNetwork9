using And9.Service.Core.Abstractions.Enums;
using And9.Service.Election.Abstractions.Interfaces;
using MessagePack;

namespace And9.Service.Election.Database.Models;

[MessagePackObject]
public class ElectionVote : IElectionVote
{
    [Key(0)]
    public int Id { get; set; }
    [Key(1)]
    public short ElectionId { get; set; }
    [IgnoreMember]
    public IElection? Election { get; set; } = null;
    [Key(2)]
    public Direction Direction { get; set; }
    [Key(3)]
    public int? MemberId { get; set; }
    [Key(4)]
    public bool? Voted { get; set; }
    [Key(5)]
    public int Votes { get; set; }
    [Key(6)]
    public DateTime LastChanged { get; set; }
    [Key(7)]
    public Guid ConcurrencyToken { get; set; }
}