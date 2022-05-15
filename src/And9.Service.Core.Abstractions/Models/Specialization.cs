using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Interfaces;
using MessagePack;

namespace And9.Service.Core.Abstractions.Models;

[MessagePackObject]
public class Specialization : ISpecialization
{
    [Key(0)]
    public int Id { get; set; }
    [Key(1)]
    public Direction Direction { get; set; }
    [Key(2)]
    public string Name { get; set; } = string.Empty;
    [IgnoreMember]
    public IList<MemberSpecialization> MemberSpecializations { get; set; } = new List<MemberSpecialization>();
    [Key(3)]
    public DateTime LastChanged { get; set; }
    [Key(4)]
    public Guid ConcurrencyToken { get; set; }
}