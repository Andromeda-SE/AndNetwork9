using System.ComponentModel;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Properties;

namespace And9.Service.Core.Abstractions;

[Localizable(true)]
public static class Extensions
{
    public static string GetDisplayString(this Direction value) => value switch
    {
        Direction.Reserve => Resources.Direction_Reserve,
        Direction.None => Resources.Direction_None,
        Direction.Training => Resources.Direction_Training,
        Direction.Infrastructure => Resources.Direction_Infrastructure,
        Direction.Research => Resources.Direction_Research,
        Direction.Military => Resources.Direction_Military,
        Direction.Agitation => Resources.Direction_Agitation,
        Direction.Auxiliary => Resources.Direction_Auxiliary,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static string GetDisplayString(this Rank value) => value switch
    {
        Rank.Outcast => Resources.Rank_Outcast,
        Rank.Enemy => Resources.Rank_Enemy,
        Rank.Guest => Resources.Rank_Guest,
        Rank.Diplomat => Resources.Rank_Diplomat,
        Rank.Ally => Resources.Rank_Ally,
        Rank.Auxiliary => Resources.Rank_Auxiliary,
        Rank.SeniorAuxiliary => Resources.Rank_SeniorAuxiliary,
        Rank.Candidate => Resources.Rank_Candidate,
        Rank.None => string.Empty,
        Rank.Neophyte => Resources.Rank_Neophyte,
        Rank.Trainee => Resources.Rank_Trainee,
        Rank.Assistant => Resources.Rank_Assistant,
        Rank.JuniorEmployee => Resources.Rank_JuniorEmployee,
        Rank.Employee => Resources.Rank_Employee,
        Rank.SeniorEmployee => Resources.Rank_SeniorEmployee,
        Rank.JuniorSpecialist => Resources.Rank_JuniorSpecialist,
        Rank.Specialist => Resources.Rank_Specialist,
        Rank.SeniorSpecialist => Resources.Rank_SeniorSpecialist,
        Rank.JuniorIntercessor => Resources.Rank_JuniorIntercessor,
        Rank.Intercessor => Resources.Rank_Intercessor,
        Rank.SeniorIntercessor => Resources.Rank_SeniorIntercessor,
        Rank.JuniorSentinel => Resources.Rank_JuniorSentinel,
        Rank.Sentinel => Resources.Rank_Sentinel,
        Rank.SeniorSentinel => Resources.Rank_SeniorSentinel,
        Rank.Advisor => Resources.Rank_Advisor,
        Rank.FirstAdvisor => Resources.Rank_FirstAdvisor,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static string? GetIconString(this Rank value) => value switch
    {
        Rank.Outcast => null,
        Rank.Enemy => null,
        Rank.Guest => null,
        Rank.Diplomat => null,
        Rank.Ally => null,
        Rank.Auxiliary => Resources.Rank_Auxiliary_Icon,
        Rank.SeniorAuxiliary => Resources.Rank_SeniorAuxiliary_Icon,
        Rank.Candidate => Resources.Rank_Candidate_Icon,
        Rank.None => null,
        Rank.Neophyte => Resources.Rank_Neophyte_Icon,
        Rank.Trainee => Resources.Rank_Trainee_Icon,
        Rank.Assistant => Resources.Rank_Assistant_Icon,
        Rank.JuniorEmployee => Resources.Rank_JuniorEmployee_Icon,
        Rank.Employee => Resources.Rank_Employee_Icon,
        Rank.SeniorEmployee => Resources.Rank_SeniorEmployee_Icon,
        Rank.JuniorSpecialist => Resources.Rank_JuniorSpecialist_Icon,
        Rank.Specialist => Resources.Rank_Specialist_Icon,
        Rank.SeniorSpecialist => Resources.Rank_SeniorSpecialist_Icon,
        Rank.JuniorIntercessor => Resources.Rank_JuniorIntercessor_Icon,
        Rank.Intercessor => Resources.Rank_Intercessor_Icon,
        Rank.SeniorIntercessor => Resources.Rank_SeniorIntercessor_Icon,
        Rank.JuniorSentinel => Resources.Rank_JuniorSentinel_Icon,
        Rank.Sentinel => Resources.Rank_Sentinel_Icon,
        Rank.SeniorSentinel => Resources.Rank_SeniorSentinel_Icon,
        Rank.Advisor => Resources.Rank_Advisor_Icon,
        Rank.FirstAdvisor => Resources.Rank_FirstAdvisor_Icon,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static SquadMemberLevel GetSquadMemberLevel(this IPublicMember member)
    {
        if (member.SquadNumber is null) return SquadMemberLevel.None;
        if (member.Rank is < Rank.None and >= Rank.Auxiliary) return SquadMemberLevel.Auxiliary;

        if (member.Rank > Rank.None)
        {
            if (member.IsSquadCommander) return member.SquadPartNumber == 0 ? SquadMemberLevel.Captain : SquadMemberLevel.Lieutenant;
            return SquadMemberLevel.Member;
        }

        return SquadMemberLevel.None;
    }


    public static string GetDisplayString(this SquadMemberLevel value) => value switch
    {
        SquadMemberLevel.Auxiliary => string.Empty,
        SquadMemberLevel.Member => string.Empty,
        SquadMemberLevel.Lieutenant => Resources.SquadMemberLevel_Lieutenant,
        SquadMemberLevel.Captain => Resources.SquadMemberLevel_Captain,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static string GetIconString(this SquadMemberLevel value) => value switch
    {
        SquadMemberLevel.None => string.Empty,
        SquadMemberLevel.Auxiliary => string.Empty,
        SquadMemberLevel.Member => string.Empty,
        SquadMemberLevel.Lieutenant => Resources.SquadMemberLevel_Lieutenant_Icon,
        SquadMemberLevel.Captain => Resources.SquadMemberLevel_Captain_Icon,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };
}