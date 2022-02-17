namespace And9.Service.Award.Abstractions.Enums;

public enum AwardType : sbyte
{
    LargePenalty = -25,
    MediumPenalty = -15,
    SmallPenalty = -5,
    None,
    Copper = 1,
    Bronze = 2,
    Silver = 5,
    Gold = 15,
    Sapphire = 25,
    Hero = 50,
}