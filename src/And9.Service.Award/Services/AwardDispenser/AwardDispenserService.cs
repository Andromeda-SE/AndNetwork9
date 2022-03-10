using System.Runtime.CompilerServices;
using And9.Lib.Utility;
using And9.Service.Award.Abstractions.Enums;
using And9.Service.Award.Senders;
using And9.Service.Award.Services.AwardDispenser.Strategy;

namespace And9.Service.Award.Services.AwardDispenser;

public sealed class AwardDispenserService<TStrategy> : TimerService where TStrategy : IAwardDispenserStrategy
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TStrategy _strategy;

    public AwardDispenserService(TStrategy strategy, IServiceScopeFactory scopeFactory)
    {
        _strategy = strategy;
        _scopeFactory = scopeFactory;
    }

    protected override TimeSpan Interval => _strategy.CheckInterval;

    protected override async Task Process()
    {
        AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        AwardCrudSender awardCrudSender = scope.ServiceProvider.GetRequiredService<AwardCrudSender>();

        await foreach ((int memberId, AwardType awardType, string description) in _strategy.GetAwards().ConfigureAwait(false))
            await awardCrudSender.Create(new()
            {
                AutomationTag = _strategy.AutomationId,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Description = description,
                GaveById = null,
                Type = awardType,
                MemberId = memberId,
                LastChanged = DateTime.UtcNow,
                ConcurrencyToken = Guid.NewGuid(),
            }).ConfigureAwait(false);
    }
}