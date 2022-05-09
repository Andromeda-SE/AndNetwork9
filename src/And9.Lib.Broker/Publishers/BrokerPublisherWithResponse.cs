using System.Collections.Concurrent;
using MessagePack;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Publishers;

internal sealed class BrokerPublisherWithResponse<TRequest, TResponse> : BaseBrokerPublisher
{
    private readonly ConcurrentDictionary<Guid, TResponse> _responses = new();
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _semaphores = new();
    public BrokerPublisherWithResponse(IModel model, ILogger<BaseBrokerPublisher> logger) : base(model, logger) { }

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
        if (properties.Headers?["Success"] is false) throw new(MessagePackSerializer.Deserialize<string>(properties.Headers["Exception"] as byte[]));
        TResponse response = MessagePackSerializer.Deserialize<TResponse>(body);
        _responses.AddOrUpdate(guid, response, (_, _) => response);
        _semaphores[guid].Release();
        _semaphores.Remove(guid, out SemaphoreSlim? semaphore);
        semaphore?.Release();
        return Task.CompletedTask;
    }

    public async ValueTask<TResponse> CallAsync(string queueName, TRequest arg, CancellationToken token = default)
    {
        Guid guid = Guid.NewGuid();
        Logger.LogInformation($"Init {guid}…");
        byte[] bytes = MessagePackSerializer.Serialize(arg, cancellationToken: token);

        IBasicProperties properties = Model.CreateBasicProperties();
        properties.ReplyTo = ReplyQueueName;
        properties.CorrelationId = guid.ToString("N");
        properties.AppId = AppId;
        Logger.LogInformation($"End init {guid}");

        SemaphoreSlim semaphore = new(0, 1);
        _semaphores.AddOrUpdate(guid,
            semaphore,
            (_, _) => throw new DuplicateWaitObjectException(guid.ToString()));

        try
        {
            Logger.LogInformation($"Send {guid}");
            Model.BasicPublish(string.Empty, queueName, properties, bytes);
            using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token, CancellationTokenSource.Token);
            if (!await semaphore.WaitAsync(30000, cts.Token).ConfigureAwait(true))
            {
                Logger.LogWarning($"Timeout {guid}");
                throw new TimeoutException();
            }

            if (!_responses.Remove(guid, out TResponse? response)) throw new KeyNotFoundException(guid.ToString());
            return response;
        }
        finally
        {
            _semaphores.Remove(guid, out _);
            semaphore.Dispose();
            Logger.LogInformation($"Finish {guid}");
        }
    }
}