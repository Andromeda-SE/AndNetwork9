using And9.Service.Award.Abstractions.Enums;
using And9.Service.Award.Abstractions.Properties;

namespace And9.Service.Award.Abstractions;

public static class Extensions
{
    public static string GetDisplayString(this AwardType value) => value switch
    {
        AwardType.LargePenalty => Resources.Award_LargePenalty,
        AwardType.MediumPenalty => Resources.Award_MediumPenalty,
        AwardType.SmallPenalty => Resources.Award_SmallPenalty,
        AwardType.None => Resources.Award_None,
        AwardType.Copper => Resources.Award_Copper,
        AwardType.Bronze => Resources.Award_Bronze,
        AwardType.Silver => Resources.Award_Silver,
        AwardType.Gold => Resources.Award_Gold,
        AwardType.Sapphire => Resources.Award_Sapphire,
        AwardType.Hero => Resources.Award_Hero,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static string GetIconString(this AwardType value) => value switch
    {
        AwardType.LargePenalty => Resources.AwardType_LargePenalty_Icon,
        AwardType.MediumPenalty => Resources.AwardType_MediumPenalty_Icon,
        AwardType.SmallPenalty => Resources.AwardType_SmallPenalty_Icon,
        AwardType.None => string.Empty,
        AwardType.Copper => Resources.AwardType_Copper_Icon,
        AwardType.Bronze => Resources.AwardType_Bronze_Icon,
        AwardType.Silver => Resources.AwardType_Silver_Icon,
        AwardType.Gold => Resources.AwardType_Gold_Icon,
        AwardType.Sapphire => Resources.AwardType_Sapphire_Icon,
        AwardType.Hero => Resources.AwardType_Hero_Icon,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };
}