using System.Reflection;

namespace And9.Lib.Broker.Senders;

public abstract class BrokerSenderWithoutResponse<TRequest>
{
    private readonly BrokerManager _brokerManager;
    private readonly string _queueName;

    protected BrokerSenderWithoutResponse(BrokerManager brokerManager)
    {
        _brokerManager = brokerManager;
        _queueName = GetType().GetCustomAttribute<QueueNameAttribute>()?.QueueName ?? throw new InvalidOperationException();
    }

    public async ValueTask CallAsync(TRequest request, CancellationToken token = default)
    {
        await _brokerManager.CallWithoutResponse(_queueName, request, token).ConfigureAwait(false);
    }
}