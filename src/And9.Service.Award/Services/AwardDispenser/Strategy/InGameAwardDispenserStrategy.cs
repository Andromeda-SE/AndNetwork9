using System.Runtime.CompilerServices;
using And9.Integration.Steam.Senders;
using And9.Integration.Steam.Senders.Models;
using And9.Service.Award.Abstractions.Enums;
using And9.Service.Award.Database;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders;

namespace And9.Service.Award.Services.AwardDispenser.Strategy;

public class InGameAwardDispenserStrategy : IAwardDispenserStrategy
{
    private const int _COOLDOWN_DAYS = 14;
    private readonly ILogger<InGameAwardDispenserStrategy> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public InGameAwardDispenserStrategy(
        ILogger<InGameAwardDispenserStrategy> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public int AutomationId => 1;
    public TimeSpan CheckInterval => TimeSpan.FromMinutes(3);

    public async IAsyncEnumerable<(int MemberId, AwardType AwardType, string Description)> GetAwards()
    {
        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        PlayerActivitySender playerActivitySender = scope.ServiceProvider.GetRequiredService<PlayerActivitySender>();
        MemberCrudSender memberCrudSender = scope.ServiceProvider.GetRequiredService<MemberCrudSender>();

        int todayDayNumber = DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
        IAsyncEnumerable<ulong> ids = memberCrudSender.ReadAll(CancellationToken.None)
            .Where(x => x.Rank is > Rank.None and < Rank.Advisor && x.SteamId is not null)
            .Select(x => x.SteamId!.Value);
        PlayerActivityResultNode[]? activityResult = await playerActivitySender.CallAsync(await ids.ToArrayAsync().ConfigureAwait(false)).ConfigureAwait(false);
        if (activityResult is null)
        {
            _logger.LogWarning("activityResult is null");
            yield break;
        }

        AwardDataContext awardDataContext = scope.ServiceProvider.GetRequiredService<AwardDataContext>();
        foreach (PlayerActivityResultNode result in activityResult.Where(x => x.InSpaceEngineers))
        {
            Member? member = await memberCrudSender.ReadBySteamId(result.SteamId).ConfigureAwait(false);
            if (member is null)
            {
                _logger.LogError("Member with SteamID «{0}» not found", result.SteamId);
                continue;
            }

            if (await awardDataContext.Awards
                    .Where(x => x.MemberId == member.Id && x.AutomationTag == AutomationId)
                    .ToAsyncEnumerable()
                    .AnyAsync(x => todayDayNumber - x.Date.DayNumber < _COOLDOWN_DAYS).ConfigureAwait(false)) continue;
            yield return (member.Id, AwardType.Copper, "Игра в Space Engineers");
        }
    }
}