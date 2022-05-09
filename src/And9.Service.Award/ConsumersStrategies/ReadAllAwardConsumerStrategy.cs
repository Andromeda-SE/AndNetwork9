using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Award.Database;
using And9.Service.Award.Senders;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Award.ConsumersStrategies;

[QueueName(ReadAllAwardsSender.QUEUE_NAME)]
public class ReadAllAwardConsumerStrategy : IBrokerConsumerWithCollectionResponseStrategy<int, Abstractions.Models.Award>
{
    private readonly AwardDataContext _awardDataContext;

    public ReadAllAwardConsumerStrategy(AwardDataContext awardDataContext) => _awardDataContext = awardDataContext;

    public async IAsyncEnumerable<Abstractions.Models.Award> ExecuteAsync(int arg)
    {
        await foreach (Abstractions.Models.Award award in _awardDataContext.Awards.AsNoTracking().ToAsyncEnumerable().ConfigureAwait(false)) yield return award;
    }
}