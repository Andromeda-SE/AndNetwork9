using And9.Gateway.Clan.Hubs;
using And9.Gateway.Clan.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Election.API.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace And9.Gateway.Clan.ConsumerStrategies;

[QueueName(RaiseElectionUpdateSender.QUEUE_NAME)]
public class RaiseElectionUpdateConsumerStrategy : IBrokerConsumerWithoutResponseStrategy<int>
{
    private readonly IHubContext<ElectionHub> _electionHub;

    public RaiseElectionUpdateConsumerStrategy(IHubContext<ElectionHub> electionHub) => _electionHub = electionHub;

    public async ValueTask ExecuteAsync(int request)
    {
        await _electionHub.Clients.All.SendAsync(nameof(IElectionClientMethods.ElectionUpdated), request).ConfigureAwait(false);
    }
}