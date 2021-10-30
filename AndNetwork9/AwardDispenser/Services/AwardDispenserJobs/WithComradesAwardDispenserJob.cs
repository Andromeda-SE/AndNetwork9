using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.AwardDispenser.Services.AwardDispenserJobs.Interfaces;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend.Senders.Steam;
using AndNetwork9.Shared.Enums;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.AwardDispenser.Services.AwardDispenserJobs;

public class WithComradesAwardDispenserJob : IAwardDispenserJob
{
    private const int _AUTOMATION_TAG = 2;
    private const int _COOLDOWN_DAYS = 14;

    public int AutomationTag => _AUTOMATION_TAG;

    public AwardType AwardType => AwardType.Copper;

    public string Description => "Совместная игра с другими участниками клана";

    public IReadOnlyCollection<PlayerActivityResultNode>? PlayerActivity { get; set; }

    public Task<IReadOnlyDictionary<Member, bool>> AvailableAsync(IEnumerable<Member> members)
    {
        Dictionary<Member, bool> result = members.ToDictionary(x => x, _ => false);
        if (PlayerActivity is null) throw new();
        PlayerActivityResultNode[]? playerActivity = PlayerActivity.Where(x => x.InSpaceEngineers).ToArray();
        ILookup<ulong?, PlayerActivityResultNode> server = playerActivity.ToLookup(x => x.GameServerSteamId);
        ILookup<ulong?, PlayerActivityResultNode> lobby = playerActivity.ToLookup(x => x.LobbySteamId);
        foreach (IGrouping<ulong?, PlayerActivityResultNode> grouping in server.Where(x => x.Key is not null))
            ApplyGrouping(grouping);
        foreach (IGrouping<ulong?, PlayerActivityResultNode> grouping in lobby.Where(x => x.Key is not null))
            ApplyGrouping(grouping);

        return Task.FromResult((IReadOnlyDictionary<Member, bool>)result);

        void ApplyGrouping(IEnumerable<PlayerActivityResultNode> grouping)
        {
            PlayerActivityResultNode[] values = grouping.ToArray();
            if (values.Length <= 1) return;
            foreach (PlayerActivityResultNode value in values)
            {
                Member member = result.Keys.First(x => x.SteamId == value.SteamId);
                result[member] = Available(member);
            }
        }
    }

    private static bool Available(Member member)
    {
        return !member.Awards.Any(x =>
            x.AutomationTag == _AUTOMATION_TAG
            && DateOnly.FromDateTime(DateTime.Today).DayNumber - x.Date.DayNumber < _COOLDOWN_DAYS);
    }
}