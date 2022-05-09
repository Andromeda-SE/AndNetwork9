using System.Reflection;

namespace And9.Lib.Broker.Senders;

public abstract class BrokerSenderWithResponse<TRequest, TResponse>
{
    private readonly BrokerManager _brokerManager;
    private readonly string _queueName;

    protected BrokerSenderWithResponse(BrokerManager brokerManager)
    {
        _brokerManager = brokerManager;
        _queueName = GetType().GetCustomAttribute<QueueNameAttribute>()?.QueueName ?? throw new InvalidOperationException();
    }

    public async ValueTask<TResponse> CallAsync(TRequest request, CancellationToken token = default) => await _brokerManager.CallWithResponse<TRequest, TResponse>(_queueName, request, token).ConfigureAwait(false);
}