using And9.Lib.Models.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Crud.Listener;

internal class ReadListener<T> : BaseRabbitListenerWithResponse<int, T> where T : IId?
{
    private readonly Func<int, Task<T>> _readFunc;

    public ReadListener(IConnection connection, string queue, ILogger<BaseRabbitListenerWithResponse<int, T>> logger, Func<int, Task<T>> readFunc) : base(connection, queue + ".Read", logger) => _readFunc = readFunc;

    protected override async Task<T> GetResponseAsync(int request) => await _readFunc(request).ConfigureAwait(false);
}