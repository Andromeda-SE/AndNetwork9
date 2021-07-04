using System;
using System.Threading.Tasks;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace AndNetwork9.Discord.Permissions
{
    public class SquadMemberPermission : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
            CommandInfo command, IServiceProvider services)
        {
            ClanDataContext data = (ClanDataContext)services.GetService(typeof(ClanDataContext))!;
            if (data is null) throw new ArgumentException("ClanContext is null", nameof(services));
            Member? member = await data.Members.FirstOrDefaultAsync(x => x.DiscordId == context.User.Id);
            return member?.Squad is not null
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError("Доступ запрещен");
        }
    }
}