using System;
using System.Collections.Generic;
using System.Linq;
using AndNetwork9.Discord.Services;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend.Discord.Channels;
using AndNetwork9.Shared.Extensions;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Direction = AndNetwork9.Shared.Enums.Direction;

namespace AndNetwork9.Discord.Extensions;

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

    public static Color? GetDiscordColor(this Direction department)
    {
        return department switch
        {
            Direction.Reserve => Color.DarkerGrey,
            Direction.None => null,
            Direction.Training => Color.Gold,
            Direction.Infrastructure => Color.Orange,
            Direction.Research => Color.Green,
            Direction.Military => Color.Blue,
            Direction.Agitation => Color.Purple,
            Direction.Auxiliary => new Color(0x00FFFFu),
            _ => throw new ArgumentOutOfRangeException(nameof(department), department, null),
        };
    }

    public static string GetLocalizedString(this CommandError commandError)
    {
        return commandError switch
        {
            CommandError.UnknownCommand => "Неизвестная команда",
            CommandError.ParseFailed => "Невеный формат аргументов",
            CommandError.BadArgCount => "Неверное количество аргументов",
            CommandError.ObjectNotFound => "Объект не найден",
            CommandError.MultipleMatches => "Неоднозначный вызов команды",
            CommandError.UnmetPrecondition => "Доступ запрещен",
            CommandError.Exception => "Ошибка при выполнении команды",
            CommandError.Unsuccessful => "Команда не была успешно выполнена",
            _ => throw new ArgumentOutOfRangeException(nameof(commandError), commandError, null),
        };
    }

    public static IEnumerable<string> GetStrings(this IEnumerable<Award> awards)
    {
        return awards.OrderBy(x => x).Select(x =>
            string.IsNullOrWhiteSpace(x.Description)
                ? $"{x.Type.GetAwardSymbol()} [{x.Date:d}]"
                : $"{x.Type.GetAwardSymbol()} [{x.Date:d}]: {x.Description}");
    }

    internal static IEnumerable<Overwrite> ToOverwrites(this Channel channel, RoleManager roleManager)
    {
        yield return new Overwrite(roleManager.EveryoneRole.Id,
            PermissionTarget.Role,
            channel.EveryonePermissions.ToOverwritePermissions());
        yield return new Overwrite(roleManager.DefaultRole.Id,
            PermissionTarget.Role,
            channel.MemberPermissions.ToOverwritePermissions());
        yield return new Overwrite(roleManager.AdvisorRole.Id,
            PermissionTarget.Role,
            channel.AdvisorPermissions.ToOverwritePermissions());

        if (channel.Squad?.DiscordRoleId is not null)
        {
            yield return new(channel.Squad.DiscordRoleId.Value,
                PermissionTarget.Role,
                channel.SquadPermissions.ToOverwritePermissions());
            foreach (Member member in channel.Squad.SquadParts.SelectMany(x => x.Members)
                         .Where(x => x.SquadPartNumber == 0 && x.SquadPart.CommanderId == x.Id && x.DiscordId.HasValue))
                yield return new(member.DiscordId!.Value,
                    PermissionTarget.User,
                    channel.SquadCommandersPermissions.ToOverwritePermissions());
        }

        if (channel.SquadPart?.DiscordRoleId is not null)
        {
            yield return new(channel.SquadPart.DiscordRoleId.Value,
                PermissionTarget.Role,
                channel.SquadPermissions.ToOverwritePermissions());
            if (channel.SquadPart.Commander.DiscordId is not null)
                yield return new(channel.SquadPart.Commander.DiscordId!.Value,
                    PermissionTarget.User,
                    channel.SquadCommandersPermissions.ToOverwritePermissions());
        }
    }

    public static OverwritePermissions ToOverwritePermissions(
        this Shared.Backend.Discord.Enums.Permissions permissionsFlags) =>
        new((ulong)permissionsFlags, ~(ulong)permissionsFlags);
}