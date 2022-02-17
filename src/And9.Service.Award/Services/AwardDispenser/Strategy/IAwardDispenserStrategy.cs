using And9.Service.Award.Abstractions.Enums;

namespace And9.Service.Award.Services.AwardDispenser.Strategy;

public interface IAwardDispenserStrategy
{
    int AutomationId { get; }
    TimeSpan CheckInterval { get; }
    IAsyncEnumerable<(int MemberId, AwardType AwardType, string Description)> GetAwards();
}