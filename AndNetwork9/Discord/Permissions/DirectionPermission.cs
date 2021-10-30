using System;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Enums;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace AndNetwork9.Discord.Permissions;

public class DirectionPermission : PreconditionAttribute
{
    private readonly Direction[] _directions;

    public DirectionPermission(params Direction[] directions)
    {
        _directions = directions;
    }

    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
        CommandInfo command, IServiceProvider services)
    {
        ClanDataContext data = (ClanDataContext)services.GetService(typeof(ClanDataContext))!;
        if (data is null) throw new ArgumentException("ClanContext is null", nameof(services));
        Member? member = await data.Members.FirstOrDefaultAsync(x => x.DiscordId == context.User.Id)
            .ConfigureAwait(false);
        return member is not null && _directions.Any(x => x == member.Direction)
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError("Доступ запрещен");
    }
}