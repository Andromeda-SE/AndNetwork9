using System.Reflection;
using System.Runtime.CompilerServices;

namespace And9.Lib.Broker.Senders;

public abstract class BrokerSenderWithCollectionResponse<TRequest, TResponse>
{
    private readonly BrokerManager _brokerManager;
    private readonly string _queueName;

    protected BrokerSenderWithCollectionResponse(BrokerManager brokerManager)
    {
        _brokerManager = brokerManager;
        _queueName = GetType().GetCustomAttribute<QueueNameAttribute>()?.QueueName ?? throw new InvalidOperationException();
    }

    public async IAsyncEnumerable<TResponse> CallAsync(TRequest request, [EnumeratorCancellation] CancellationToken token = default)
    {
        await foreach (TResponse item in _brokerManager.CallWithCollectionResponse<TRequest, TResponse>(_queueName, request, token).ConfigureAwait(false)) yield return item;
    }
}