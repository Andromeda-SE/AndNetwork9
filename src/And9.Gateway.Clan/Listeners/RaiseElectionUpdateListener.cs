using And9.Gateway.Clan.Hubs;
using And9.Gateway.Clan.Senders;
using And9.Lib.Broker;
using And9.Service.Election.API.Interfaces;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;

namespace And9.Gateway.Clan.Listeners;

public class RaiseElectionUpdateListener : BaseRabbitListenerWithoutResponse<int>
{
    private readonly IHubContext<ElectionHub> _electionHub;
    public RaiseElectionUpdateListener(IConnection connection, ILogger<BaseRabbitListenerWithoutResponse<int>> logger, IHubContext<ElectionHub> electionHub) 
        : base(connection, RaiseElectionUpdateSender.QUEUE_NAME, logger)
    {
        _electionHub = electionHub;
    }

    public override async Task Run(int request)
    {
        await _electionHub.Clients.All.SendAsync(nameof(IElectionClientMethods.ElectionUpdated), request).ConfigureAwait(false);
    }
}