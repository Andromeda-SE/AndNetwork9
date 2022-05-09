using System.Collections.Concurrent;
using MessagePack;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Publishers;

internal sealed class BrokerPublisherWithoutResponse<TRequest> : BaseBrokerPublisher
{
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _semaphores = new();
    public BrokerPublisherWithoutResponse(IModel model, ILogger<BaseBrokerPublisher> logger) : base(model, logger) { }

    public override Task HandleBasicDeliver(
        string consumerTag,
        ulong deliveryTag,
        bool redelivered,
        string exchange,
        string routingKey,
        IBasicProperties properties,
        ReadOnlyMemory<byte> body)
    {
        Guid guid = Guid.Parse(properties.CorrelationId);
        _semaphores.Remove(guid, out SemaphoreSlim? semaphore);
        semaphore?.Release();
        return Task.CompletedTask;
    }

    public async ValueTask CallAsync(string queueName, TRequest arg, CancellationToken token = default)
    {
        Guid guid = Guid.NewGuid();
        Logger.LogInformation($"Init {guid}…");
        byte[] bytes = MessagePackSerializer.Serialize(arg);

        IBasicProperties properties = Model.CreateBasicProperties();
        properties.ReplyTo = ReplyQueueName;
        properties.CorrelationId = guid.ToString("N");
        properties.AppId = AppId;
        Logger.LogInformation($"End init {guid}");

        using SemaphoreSlim semaphore = new(1, 1);
        _semaphores.AddOrUpdate(guid,
            semaphore,
            (_, _) => throw new DuplicateWaitObjectException(guid.ToString()));
        Logger.LogInformation($"Send {guid}");
        Model.BasicPublish(string.Empty, queueName, properties, bytes);
        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token, CancellationTokenSource.Token);
        if (!await semaphore.WaitAsync(30000, cts.Token).ConfigureAwait(false))
        {
            Logger.LogWarning($"Timeout {guid}");
            throw new TimeoutException();
        }
        Logger.LogInformation($"Finish {guid}");
    }
}