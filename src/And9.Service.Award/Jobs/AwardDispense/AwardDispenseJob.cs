using And9.Service.Award.Abstractions.Enums;
using And9.Service.Award.Jobs.AwardDispense.Strategy;
using And9.Service.Award.Senders;
using Quartz;

namespace And9.Service.Award.Jobs.AwardDispense;

[DisallowConcurrentExecution]
public sealed class AwardDispenseJob<TStrategy> : IJob where TStrategy : IAwardDispenserStrategy
{
    private readonly CreateAwardSender _createAwardSender;
    private readonly TStrategy _strategy;

    public AwardDispenseJob(TStrategy strategy, CreateAwardSender createAwardSender)
    {
        _strategy = strategy;
        _createAwardSender = createAwardSender;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await Task.WhenAll(await _strategy.GetAwards().Select(x =>
        {
            (int memberId, AwardType awardType, string description) = x;
            return _createAwardSender.CallAsync(new()
            {
                AutomationTag = _strategy.AutomationId,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Description = description,
                GaveById = null,
                Type = awardType,
                MemberId = memberId,
                LastChanged = DateTime.UtcNow,
                ConcurrencyToken = Guid.NewGuid(),
            }).AsTask();
        }).ToArrayAsync().ConfigureAwait(false)).ConfigureAwait(false);
    }
}