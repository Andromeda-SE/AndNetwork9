using And9.Lib.Models.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Enums;

namespace And9.Service.Core.Abstractions.Interfaces;

public interface IPublicMember : IId
{
    string Nickname { get; set; }
    string? RealName { get; set; }
    ulong? DiscordId { get; }
    Rank Rank { get; }
    Direction Direction { get; set; }
    bool IsSquadCommander { get; }
    short? SquadNumber { get; }
    short SquadPartNumber { get; }
    /*SquadMemberLevel SquadMemberLevel =>
        SquadNumber.HasValue
            ? IsSquadCommander
                ? SquadPartNumber == 0
                    ? SquadMemberLevel.Captain
                    : SquadMemberLevel.Lieutenant
                : Rank > Rank.None
                    ? SquadMemberLevel.Member
                    : SquadMemberLevel.Auxiliary
            : SquadMemberLevel.None;*/
}