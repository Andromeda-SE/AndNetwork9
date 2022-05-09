using And9.Service.Award.Abstractions.Enums;

namespace And9.Service.Award.Jobs.AwardDispense.Strategy;

public interface IAwardDispenserStrategy
{
    int AutomationId { get; }
    IAsyncEnumerable<(int MemberId, AwardType AwardType, string Description)> GetAwards();
}