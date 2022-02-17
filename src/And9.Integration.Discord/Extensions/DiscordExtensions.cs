using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Properties;
using Discord;
using Discord.Commands;

namespace And9.Integration.Discord.Extensions;

public static class DiscordExtensions
{
    public static LogLevel ToLogLevel(this LogSeverity severity)
    {
        return severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null),
        };
    }

    public static string GetLocalizedString(this CommandError commandError)
    {
        return commandError switch
        {
            CommandError.UnknownCommand => Resources.CommandError_UnknownCommand,
            CommandError.ParseFailed => Resources.CommandError_ParseFailed,
            CommandError.BadArgCount => Resources.CommandError_BadArgCount,
            CommandError.ObjectNotFound => Resources.CommandError_ObjectNotFound,
            CommandError.MultipleMatches => Resources.CommandError_MultipleMatches,
            CommandError.UnmetPrecondition => Resources.CommandError_UnmetPrecondition,
            CommandError.Exception => Resources.CommandError_Exception,
            CommandError.Unsuccessful => Resources.CommandError_Unsuccessful,
            _ => throw new ArgumentOutOfRangeException(nameof(commandError), commandError, null),
        };
    }

    public static OverwritePermissions ToOverwritePermissions(
        this DiscordPermissions permissionsFlags) =>
        new((ulong)permissionsFlags, ~(ulong)permissionsFlags);

    public static GuildPermissions ToGuildPermissions(this DiscordPermissions permissionsFlags) => new((ulong)permissionsFlags);
}