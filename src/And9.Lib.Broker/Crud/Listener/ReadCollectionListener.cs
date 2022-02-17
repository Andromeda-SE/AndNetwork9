using And9.Lib.Models.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Crud.Listener;

public class ReadCollectionListener<T> : BaseRabbitListenerWithStreamResponse<object, T> where T : IId
{
    private readonly Func<object, CancellationToken, IAsyncEnumerable<T>> _readAllFunc;

    public ReadCollectionListener(
        IConnection connection,
        string queue,
        string queueSuffix,
        ILogger<BaseRabbitListenerWithStreamResponse<object, T>> logger,
        Func<object, CancellationToken, IAsyncEnumerable<T>> readAllFunc)
        : base(connection, queue + "." + queueSuffix, logger)
        => _readAllFunc = readAllFunc;

    protected override IAsyncEnumerable<T> GetResponseAsync(object arg, CancellationToken cancellationToken = default) => _readAllFunc(arg, cancellationToken);
}