using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Crud.Listener;

internal class CreateListener<T> : BaseRabbitListenerWithResponse<T, int>
{
    private readonly Func<T, Task<int>> _createAction;

    public CreateListener(IConnection connection, string queue, ILogger<BaseRabbitListenerWithResponse<T, int>> logger, Func<T, Task<int>> createAction) : base(connection, queue + ".Create", logger) => _createAction = createAction;

    protected override async Task<int> GetResponseAsync(T request) => await _createAction(request).ConfigureAwait(false);
}