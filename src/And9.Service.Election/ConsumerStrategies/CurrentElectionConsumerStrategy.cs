using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Election.Database;
using And9.Service.Election.Senders;

namespace And9.Service.Election.ConsumerStrategies;

[QueueName(CurrentElectionSender.QUEUE_NAME)]
public class CurrentElectionConsumerStrategy : IBrokerConsumerWithCollectionResponseStrategy<int, Abstractions.Models.Election>
{
    private readonly ElectionDataContext _electionDataContext;

    public CurrentElectionConsumerStrategy(ElectionDataContext electionDataContext) => _electionDataContext = electionDataContext;

    public async IAsyncEnumerable<Abstractions.Models.Election> ExecuteAsync(int arg)
    {
        await foreach (Abstractions.Models.Election election in
                       _electionDataContext.GetCurrentElectionsWithVotesAsync().ConfigureAwait(false))
            yield return election;
    }
}