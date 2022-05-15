namespace And9.Integration.Discord.Abstractions.Enums;

public enum DiscordRoleScope
{
    Member,
    SquadPart,
    Squad,
    [Obsolete("Directions is canceled", true)]
    Direction,
    Advisor,
}