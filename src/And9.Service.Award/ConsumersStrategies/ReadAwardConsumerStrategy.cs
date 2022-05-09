using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Service.Award.Database;
using And9.Service.Award.Senders;

namespace And9.Service.Award.ConsumersStrategies;

[QueueName(ReadAwardSender.QUEUE_NAME)]
public class ReadAwardConsumerStrategy : IBrokerConsumerWithResponseStrategy<int, Abstractions.Models.Award?>
{
    private readonly AwardDataContext _awardDataContext;
    public ReadAwardConsumerStrategy(AwardDataContext awardDataContext) => _awardDataContext = awardDataContext;

    public async ValueTask<Abstractions.Models.Award?> ExecuteAsync(int id) =>
        await _awardDataContext.Awards.FindAsync(id).ConfigureAwait(false);
}