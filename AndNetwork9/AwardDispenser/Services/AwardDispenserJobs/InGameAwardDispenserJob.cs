using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.AwardDispenser.Services.AwardDispenserJobs.Interfaces;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend.Senders.Steam;
using AndNetwork9.Shared.Enums;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.AwardDispenser.Services.AwardDispenserJobs
{
    public class InGameAwardDispenserJob : IAwardDispenserJob
    {
        private const int _AUTOMATION_TAG = 1;
        private const int _COOLDOWN_DAYS = 14;

        public int AutomationTag => _AUTOMATION_TAG;
        public AwardType AwardType => AwardType.Copper;
        public string Description => "Нахождение в игре";

        public IReadOnlyCollection<PlayerActivityResultNode>? PlayerActivity { get; set; }

        public Task<IReadOnlyDictionary<Member, bool>> AvailableAsync(IEnumerable<Member> members)
        {
            Dictionary<Member, bool> result = members.ToDictionary(x => x, _ => false);
            if (PlayerActivity is null) throw new();
            foreach ((Member member, PlayerActivityResultNode activity) in
                result.Keys.Join(PlayerActivity,
                    x => x.SteamId,
                    x => x.SteamId,
                    (member, activity) => (member, activity)))
                result[member] = Available(member) && activity.InSpaceEngineers;
            return Task.FromResult((IReadOnlyDictionary<Member, bool>)result);
        }

        private bool Available(Member member) => !member.Awards.Any(x =>
            x.AutomationTag == _AUTOMATION_TAG
            && DateOnly.FromDateTime(DateTime.Today).DayNumber - x.Date.DayNumber < _COOLDOWN_DAYS);
    }
}