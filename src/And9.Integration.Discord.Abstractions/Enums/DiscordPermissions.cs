namespace And9.Integration.Discord.Abstractions.Enums;

public enum DiscordPermissions : ulong
{
    None = 0,

    View = 1024UL,
    Read = 1115136UL,
    Write = 380141424192UL,
    Priority = 388748795840UL,
    Moderator = 393056354240UL,

    All = ulong.MaxValue,
}