using System.Runtime.CompilerServices;
using And9.Lib.Broker.Publishers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace And9.Lib.Broker;

public sealed class BrokerManager : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private AsyncServiceScope _scope;

    public BrokerManager(IServiceScopeFactory serviceScopeFactory) => _serviceScopeFactory = serviceScopeFactory;

    internal IDictionary<string, Type> Consumers { private get; init; }
    internal IDictionary<string, Func<IServiceScope, BaseBrokerPublisher>> Publishers { private get; init; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _scope = _serviceScopeFactory.CreateAsyncScope();

        foreach ((string? queueName, Type consumerType) in Consumers)
        {
            AsyncServiceScope scope = _scope.ServiceProvider.CreateAsyncScope();
            IModel model = scope.ServiceProvider.GetRequiredService<IModel>();
            model.QueueDeclare(queueName, exclusive: false, autoDelete: false);
            object consumer = scope.ServiceProvider.GetRequiredService(consumerType);
            model.BasicConsume((AsyncDefaultBasicConsumer)consumer, queueName);
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _scope.DisposeAsync().ConfigureAwait(false);
        _scope = default;
    }

    internal async ValueTask CallWithoutResponse<TArg>(string queueName, TArg arg, CancellationToken token = default)
    {
        if (!Publishers.TryGetValue(queueName, out Func<IServiceScope, BaseBrokerPublisher>? basePublisherFactory)) throw new KeyNotFoundException();
        if (basePublisherFactory.Invoke(_scope) is not BrokerPublisherWithoutResponse<TArg> publisher) throw new InvalidOperationException();
        IModel model = _scope.ServiceProvider.GetRequiredService<IModel>();
        model.QueueDeclarePassive(queueName);
        try
        {
            await publisher.CallAsync(queueName, arg, token).ConfigureAwait(false);
        }
        finally
        {
            publisher.Dispose();
        }
    }

    internal async ValueTask<TResponse> CallWithResponse<TArg, TResponse>(string queueName, TArg arg, CancellationToken token = default)
    {
        if (!Publishers.TryGetValue(queueName, out Func<IServiceScope, BaseBrokerPublisher>? basePublisherFactory)) throw new KeyNotFoundException();
        if (basePublisherFactory.Invoke(_scope) is not BrokerPublisherWithResponse<TArg, TResponse> publisher) throw new InvalidOperationException();
        IModel model = _scope.ServiceProvider.GetRequiredService<IModel>();
        model.QueueDeclarePassive(queueName);
        try
        {
            return await publisher.CallAsync(queueName, arg, token).ConfigureAwait(false);
        }
        finally
        {
            publisher.Dispose();
        }
    }

    internal async IAsyncEnumerable<TResponse> CallWithCollectionResponse<TArg, TResponse>(string queueName, TArg arg, [EnumeratorCancellation] CancellationToken token = default)
    {
        if (!Publishers.TryGetValue(queueName, out Func<IServiceScope, BaseBrokerPublisher>? basePublisher)) throw new KeyNotFoundException();
        if (basePublisher.Invoke(_scope) is not BrokerPublisherWithCollectionResponse<TArg, TResponse> publisher) throw new InvalidOperationException();
        IModel model = _scope.ServiceProvider.GetRequiredService<IModel>();
        model.QueueDeclarePassive(queueName);
        try
        {
            await foreach (TResponse item in publisher.CallAsync(queueName, arg, token).ConfigureAwait(false)) yield return item;
        }
        finally
        {
            publisher.Dispose();
        }
    }
}