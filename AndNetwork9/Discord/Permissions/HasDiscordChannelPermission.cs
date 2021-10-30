using System;
using System.Threading.Tasks;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Discord.Channels;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace AndNetwork9.Discord.Permissions;

public class HasDiscordChannelPermission : PreconditionAttribute
{
    private readonly Shared.Backend.Discord.Enums.Permissions _permissions;

    public HasDiscordChannelPermission(
        Shared.Backend.Discord.Enums.Permissions permissions = Shared.Backend.Discord.Enums.Permissions.Read)
    {
        _permissions = permissions;
    }

    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
        CommandInfo command,
        IServiceProvider services)
    {
        ClanDataContext data = (ClanDataContext)services.GetService(typeof(ClanDataContext))!;
        if (data is null) throw new ArgumentException("ClanContext is null", nameof(services));
        Member? member = await data.Members.FirstOrDefaultAsync(x => x.DiscordId == context.User.Id)
            .ConfigureAwait(false);
        if (member is null) return PreconditionResult.FromError("Участник не найден");
        Channel? channel = await data.DiscordChannels.FindAsync(context.Channel.Id).ConfigureAwait(false);
        return channel.HasPermissionLevel(member, _permissions)
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError("Недостаточно прав на канале");
    }
}