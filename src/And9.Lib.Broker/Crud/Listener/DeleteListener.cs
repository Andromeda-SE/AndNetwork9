using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Crud.Listener;

internal class DeleteListener : BaseRabbitListenerWithoutResponse<int>
{
    private readonly Func<int, Task> _deleteFunc;

    public DeleteListener(IConnection connection, string queue, ILogger<BaseRabbitListenerWithoutResponse<int>> logger, Func<int, Task> deleteFunc) : base(connection, queue + ".Delete", logger) => _deleteFunc = deleteFunc;

    public override async Task Run(int request) => await _deleteFunc(request).ConfigureAwait(false);
}