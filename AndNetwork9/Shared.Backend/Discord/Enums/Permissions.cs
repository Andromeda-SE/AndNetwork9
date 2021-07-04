namespace AndNetwork9.Shared.Backend.Discord.Enums
{
    public enum Permissions : ulong
    {
        None = 0,

        View = 1024,
        Read = 1115136,
        Write = 36818496,
        Priority = 53727040,
        Moderator = 66318272,

        All = ulong.MaxValue,
    }
}