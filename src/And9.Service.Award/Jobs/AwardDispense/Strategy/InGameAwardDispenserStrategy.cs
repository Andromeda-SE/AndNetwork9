using And9.Integration.Steam.Senders;
using And9.Integration.Steam.Senders.Models;
using And9.Service.Award.Abstractions.Enums;
using And9.Service.Award.Database;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders;

namespace And9.Service.Award.Jobs.AwardDispense.Strategy;

public class InGameAwardDispenserStrategy : IAwardDispenserStrategy
{
    private const int _COOLDOWN_DAYS = 14;
    private readonly AwardDataContext _awardDataContext;
    private readonly ILogger<InGameAwardDispenserStrategy> _logger;
    private readonly PlayerActivitySender _playerActivitySender;
    private readonly ReadAllMembersSender _readAllMembersSender;
    private readonly ReadMemberBySteamIdSender _readMemberBySteamIdSender;

    public InGameAwardDispenserStrategy(
        ILogger<InGameAwardDispenserStrategy> logger,
        PlayerActivitySender playerActivitySender,
        ReadAllMembersSender readAllMembersSender,
        AwardDataContext awardDataContext,
        ReadMemberBySteamIdSender readMemberBySteamIdSender)
    {
        _logger = logger;
        _playerActivitySender = playerActivitySender;
        _readAllMembersSender = readAllMembersSender;
        _awardDataContext = awardDataContext;
        _readMemberBySteamIdSender = readMemberBySteamIdSender;
    }

    public int AutomationId => 1;

    public async IAsyncEnumerable<(int MemberId, AwardType AwardType, string Description)> GetAwards()
    {
        int todayDayNumber = DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
        IAsyncEnumerable<ulong> ids = _readAllMembersSender.CallAsync(0, CancellationToken.None)
            .Where(x => x.Rank is > Rank.None and < Rank.Advisor && x.SteamId is not null)
            .Select(x => x.SteamId!.Value);
        PlayerActivityResultNode[]? activityResult = await _playerActivitySender.CallAsync(await ids.ToArrayAsync().ConfigureAwait(false)).ConfigureAwait(false);
        if (activityResult is null)
        {
            _logger.LogWarning("activityResult is null");
            yield break;
        }

        foreach (PlayerActivityResultNode result in activityResult.Where(x => x.InSpaceEngineers))
        {
            Member? member = await _readMemberBySteamIdSender.CallAsync(result.SteamId).ConfigureAwait(false);
            if (member is null)
            {
                _logger.LogError("Member with SteamID «{0}» not found", result.SteamId);
                continue;
            }

            if (await _awardDataContext.Awards
                    .Where(x => x.MemberId == member.Id && x.AutomationTag == AutomationId)
                    .ToAsyncEnumerable()
                    .AnyAsync(x => todayDayNumber - x.Date.DayNumber < _COOLDOWN_DAYS).ConfigureAwait(false)) continue;
            yield return (member.Id, AwardType.Copper, "Игра в Space Engineers");
        }
    }
}