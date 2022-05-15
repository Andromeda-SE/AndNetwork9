using And9.Lib.Models.Abstractions.Interfaces;

namespace And9.Service.Core.Abstractions.Interfaces;

public interface IMemberSpecialization : IConcurrencyToken
{
    public int MemberId { get; }
    public int SpecializationId { get; }
    public int? Priority { get; }
    public DateTime? ApproveDateTime { get; }
}