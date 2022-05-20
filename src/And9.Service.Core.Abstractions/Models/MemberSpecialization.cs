using And9.Service.Core.Abstractions.Interfaces;
using MessagePack;

namespace And9.Service.Core.Abstractions.Models;

[MessagePackObject]
public class MemberSpecialization : IMemberSpecialization
{
    [IgnoreMember]
    public Member Member { get; set; }
    [IgnoreMember]
    public Specialization Specialization { get; set; }
    [Key(0)]
    public int MemberId { get; set; }
    [Key(1)]
    public int SpecializationId { get; set; }
    [Key(2)]
    public int? Priority { get; set; }
    [Key(3)]
    public DateTime? ApproveDateTime { get; set; }
    [Key(4)]
    public DateTime LastChanged { get; set; }
    [Key(5)]
    public Guid ConcurrencyToken { get; set; }
}