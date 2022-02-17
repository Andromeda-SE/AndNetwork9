using System.Runtime.CompilerServices;
using And9.Lib.Broker.Crud.Sender;
using And9.Lib.Models.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Crud;

public abstract class RabbitCrudSender<T> : IDisposable where T : IId
{
    protected RabbitCrudSender(
        IConnection connection,
        string queueName,
        CrudFlags availableActions,
        ILogger<BaseRabbitSenderWithResponse<T, int>> createLogger,
        ILogger<BaseRabbitSenderWithResponse<int, T>> readLogger,
        ILogger<BaseRabbitSenderWithStreamResponse<object, T>> readCollectionLogger,
        ILogger<BaseRabbitSenderWithResponse<T, T>> updateLogger,
        ILogger<BaseRabbitSenderWithoutResponse<int>> deleteLogger)
    {
        AvailableActions = availableActions;
        if (AvailableActions.HasFlag(CrudFlags.Create)) CreateSender = new(connection, queueName, createLogger);
        if (AvailableActions.HasFlag(CrudFlags.Read))
        {
            ReadSender = new(connection, queueName, readLogger);
            ReadAllSender = new(connection, queueName, "All", readCollectionLogger);
        }

        if (AvailableActions.HasFlag(CrudFlags.Update)) UpdateSender = new(connection, queueName, updateLogger);
        if (AvailableActions.HasFlag(CrudFlags.Delete)) DeleteSender = new(connection, queueName, deleteLogger);
    }

    public CrudFlags AvailableActions { get; }
    private CreateSender<T>? CreateSender { get; }
    private ReadSender<T>? ReadSender { get; }
    private ReadCollectionSender<T>? ReadAllSender { get; }
    private UpdateSender<T>? UpdateSender { get; }
    private DeleteSender? DeleteSender { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    public async Task<int> Create(T entity)
    {
        if (CreateSender is null) throw new NotSupportedException();
        return await CreateSender.CallAsync(entity).ConfigureAwait(false);
    }

    public async Task<T?> Update(T entity)
    {
        if (UpdateSender is null) throw new NotSupportedException();
        return await UpdateSender.CallAsync(entity).ConfigureAwait(false);
    }

    public async Task<T?> Read(int id)
    {
        if (ReadSender is null) throw new NotSupportedException();
        return await ReadSender.CallAsync(id).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<T> ReadAll([EnumeratorCancellation] CancellationToken token)
    {
        if (ReadAllSender is null) throw new NotSupportedException();
        await foreach (T item in ReadAllSender.CallAsync(new()).WithCancellation(token).ConfigureAwait(false)) yield return item;
    }

    public async Task Delete(int id)
    {
        if (DeleteSender is null) throw new NotSupportedException();
        await DeleteSender.CallAsync(id).ConfigureAwait(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            CreateSender?.Dispose();
            ReadSender?.Dispose();
            UpdateSender?.Dispose();
            DeleteSender?.Dispose();
        }
    }
}