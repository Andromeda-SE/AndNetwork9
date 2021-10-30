namespace AndNetwork9.Shared.Backend.Discord.Enums;

public enum Permissions : ulong
{
    None = 0,

    View = 1024UL,
    Read = 1115136UL,
    Write = 38691524160UL,
    Priority = 107427909440UL,
    Moderator = 107440500672UL,

    All = ulong.MaxValue,
}