using System.Collections.Generic;
using System.Threading.Tasks;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend.Senders.Steam;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.AwardDispenser.Services.AwardDispenserJobs.Interfaces
{
    public interface IAwardDispenserJob
    {
        int AutomationTag { get; }
        AwardType AwardType { get; }
        string Description { get; }
        IReadOnlyCollection<PlayerActivityResultNode>? PlayerActivity { get; set; }

        Task<IReadOnlyDictionary<Member, bool>> AvailableAsync(IEnumerable<Member> members);
    }
}