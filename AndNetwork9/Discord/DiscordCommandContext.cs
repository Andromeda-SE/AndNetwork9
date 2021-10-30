using System;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AndNetwork9.Discord;

public sealed class DiscordCommandContext : SocketCommandContext, IDisposable
{
    public DiscordCommandContext(DiscordSocketClient client, SocketUserMessage msg, IServiceScope scope,
        IDisposable typing) : base(client, msg)
    {
        Scope = scope;
        Typing = typing;
    }

    public IDisposable Typing { get; }
    public IServiceScope Scope { get; }

    public void Dispose()
    {
        Typing.Dispose();
        Scope.Dispose();
    }
}