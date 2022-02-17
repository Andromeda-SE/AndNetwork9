using And9.Lib.Broker.Crud.Listener;
using And9.Lib.Models.Abstractions.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Crud;

public abstract class RabbitCrudListener<T> : IHostedService where T : IId
{
    protected RabbitCrudListener(
        IConnection connection,
        string queueName,
        CrudFlags availableActions,
        ILogger<BaseRabbitListenerWithResponse<T, int>> createLogger,
        ILogger<BaseRabbitListenerWithResponse<int, T?>> readLogger,
        ILogger<BaseRabbitListenerWithStreamResponse<object, T>> readAllLogger,
        ILogger<BaseRabbitListenerWithResponse<T, T>> updateLogger,
        ILogger<BaseRabbitListenerWithoutResponse<int>> deleteLogger)
    {
        AvailableActions = availableActions;
        if (AvailableActions.HasFlag(CrudFlags.Create)) CreateListener = new(connection, queueName, createLogger, Create);
        if (AvailableActions.HasFlag(CrudFlags.Read))
        {
            ReadListener = new(connection, queueName, readLogger, Read);
            ReadAllListener = new(connection, queueName, "All", readAllLogger, ReadAll);
        }

        if (AvailableActions.HasFlag(CrudFlags.Update)) UpdateListener = new(connection, queueName, updateLogger, Update);
        if (AvailableActions.HasFlag(CrudFlags.Delete)) DeleteListener = new(connection, queueName, deleteLogger, Delete);
    }

    public CrudFlags AvailableActions { get; }
    private CreateListener<T>? CreateListener { get; }
    private ReadListener<T?>? ReadListener { get; }
    private ReadCollectionListener<T>? ReadAllListener { get; }
    private UpdateListener<T>? UpdateListener { get; }
    private DeleteListener? DeleteListener { get; }

    public virtual async Task StartAsync(CancellationToken cancellationToken) => await Task.WhenAll(
        CreateListener?.StartAsync(cancellationToken) ?? Task.CompletedTask,
        ReadListener?.StartAsync(cancellationToken) ?? Task.CompletedTask,
        ReadAllListener?.StartAsync(cancellationToken) ?? Task.CompletedTask,
        UpdateListener?.StartAsync(cancellationToken) ?? Task.CompletedTask,
        DeleteListener?.StartAsync(cancellationToken) ?? Task.CompletedTask
    ).ConfigureAwait(false);

    public virtual async Task StopAsync(CancellationToken cancellationToken) => await Task.WhenAll(
        CreateListener?.StopAsync(cancellationToken) ?? Task.CompletedTask,
        ReadListener?.StopAsync(cancellationToken) ?? Task.CompletedTask,
        ReadAllListener?.StopAsync(cancellationToken) ?? Task.CompletedTask,
        UpdateListener?.StopAsync(cancellationToken) ?? Task.CompletedTask,
        DeleteListener?.StopAsync(cancellationToken) ?? Task.CompletedTask
    ).ConfigureAwait(false);

    public abstract Task<int> Create(T entity);
    public abstract Task<T> Update(T entity);
    public abstract Task<T?> Read(int id);
    public abstract IAsyncEnumerable<T> ReadAll(object arg, CancellationToken cancellationToken);
    public abstract Task Delete(int id);
}