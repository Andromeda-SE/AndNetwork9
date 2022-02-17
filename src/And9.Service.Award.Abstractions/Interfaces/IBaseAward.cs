using And9.Service.Award.Abstractions.Enums;

namespace And9.Service.Award.Abstractions.Interfaces;

public interface IBaseAward
{
    public int MemberId { get; }
    public AwardType Type { get; }
    public string? Description { get; }
}