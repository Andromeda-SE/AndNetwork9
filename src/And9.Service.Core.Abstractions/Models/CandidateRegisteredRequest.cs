using And9.Service.Core.Abstractions.Interfaces;
using MessagePack;

namespace And9.Service.Core.Abstractions.Models;

[MessagePackObject]
public record CandidateRegisteredRequest : ICandidateRegisteredRequest
{
    [IgnoreMember]
    public Member Member { get; set; } = null!;
    [Key(0)]
    public int Id { get; set; }
    [Key(1)]
    public int MemberId { get; set; }
    [Key(2)]
    public int? HoursCount { get; set; }
    [Key(3)]
    public int? Age { get; set; }
    [Key(4)]
    public string? Recommendation { get; set; }
    [Key(5)]
    public string? Description { get; set; }
    [Key(6)]
    public short? AuxiliarySquad { get; set; }
    [Key(7)]
    public bool? Accepted { get; set; }
    [Key(8)]
    public DateTime LastChanged { get; set; }
    [Key(9)]
    public Guid ConcurrencyToken { get; set; }
}