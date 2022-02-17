using And9.Gateway.Clan.Auth.Attributes;
using And9.Integration.Discord.API.Interfaces;
using And9.Integration.Discord.Senders;
using And9.Service.Core.Abstractions.Enums;
using Microsoft.AspNetCore.SignalR;

namespace And9.Gateway.Clan.Hubs;

public class DiscordHub : Hub<IDiscordClientMethods>, IDiscordServerMethods
{
    private readonly SendDirectMessageSender _sendDirectMessageSender;
    public DiscordHub(SendDirectMessageSender sendDirectMessageSender) => _sendDirectMessageSender = sendDirectMessageSender;

    [MinRankAuthorize(Rank.Advisor)]
    public async Task SendDirectMessageAsync(ulong discordId, string message)
    {
        await _sendDirectMessageSender.CallAsync(new(discordId, message)).ConfigureAwait(false);
    }
}