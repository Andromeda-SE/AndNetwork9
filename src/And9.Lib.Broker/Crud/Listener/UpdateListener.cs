using And9.Lib.Models.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Crud.Listener;

internal class UpdateListener<T> : BaseRabbitListenerWithResponse<T, T> where T : IId
{
    private readonly Func<T, Task<T>> _updateFunc;

    public UpdateListener(IConnection connection, string queue, ILogger<BaseRabbitListenerWithResponse<T, T>> logger, Func<T, Task<T>> updateFunc) : base(connection, queue + ".Update", logger) => _updateFunc = updateFunc;

    protected override async Task<T> GetResponseAsync(T request) => await _updateFunc(request).ConfigureAwait(false);
}