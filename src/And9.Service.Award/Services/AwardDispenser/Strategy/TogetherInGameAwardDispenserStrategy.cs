using System.Runtime.CompilerServices;
using And9.Integration.Steam.Senders;
using And9.Integration.Steam.Senders.Models;
using And9.Service.Award.Abstractions.Enums;
using And9.Service.Award.Database;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders;

namespace And9.Service.Award.Services.AwardDispenser.Strategy;

public sealed class TogetherInGameAwardDispenserStrategy : IAwardDispenserStrategy
{
    private readonly ILogger<TogetherInGameAwardDispenserStrategy> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TogetherInGameAwardDispenserStrategy(
        ILogger<TogetherInGameAwardDispenserStrategy> logger,
        IServiceScopeFactory serviceScope)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScope;
    }

    public int AutomationId => 2;
    public TimeSpan CheckInterval => TimeSpan.FromMinutes(5);
    private const int _COOLDOWN_DAYS = 14;

    public async IAsyncEnumerable<(int MemberId, AwardType AwardType, string Description)> GetAwards()
    {
        AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
        MemberCrudSender memberCrudSender = scope.ServiceProvider.GetRequiredService<MemberCrudSender>();
        PlayerActivitySender playerActivitySender = scope.ServiceProvider.GetRequiredService<PlayerActivitySender>();

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
        foreach (IGrouping<ulong?, PlayerActivityResultNode> party in activityResult.GroupBy(x => x.GameServerSteamId).Concat(activityResult.GroupBy(x => x.LobbySteamId)))
        {
            PlayerActivityResultNode[] array = party.ToArray();
            int spaceEngineersCount = array.Count(x => x.InSpaceEngineers);
            if (spaceEngineersCount < 2) continue;
            foreach (PlayerActivityResultNode result in array)
            {
                Member? member = await memberCrudSender.ReadBySteamId(result.SteamId).ConfigureAwait(false);
                if (member is null)
                {
                    _logger.LogError("Member with SteamID «{0}» not found", result.SteamId);
                    continue;
                }

                if (awardDataContext.Awards.Any(x => x.MemberId == member.Id && x.AutomationTag == AutomationId && todayDayNumber - x.Date.DayNumber >= _COOLDOWN_DAYS))
                    yield return (member.Id, AwardType.Copper, "Совместая игра с другими участниками клана");
            }
        }
    }
}