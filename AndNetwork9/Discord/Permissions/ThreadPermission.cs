using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace AndNetwork9.Discord.Permissions;

public class ThreadPermission : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command,
        IServiceProvider services) => Task.FromResult(context.Channel is RestThreadChannel or SocketThreadChannel
        ? PreconditionResult.FromSuccess()
        : PreconditionResult.FromError("Команду можно использовать только в ветках (threads)"));
}