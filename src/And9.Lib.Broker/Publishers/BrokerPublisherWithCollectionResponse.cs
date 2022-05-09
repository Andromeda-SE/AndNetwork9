using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using MessagePack;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Publishers;

internal sealed class BrokerPublisherWithCollectionResponse<TRequest, TResponse> : BaseBrokerPublisher
{
    private readonly ConcurrentDictionary<Guid, BufferBlock<TResponse>> _responses = new();
    public BrokerPublisherWithCollectionResponse(IModel model, ILogger<BaseBrokerPublisher> logger) : base(model, logger) { }

    public override async Task HandleBasicDeliver(
        string consumerTag,
        ulong deliveryTag,
        bool redelivered,
        string exchange,
        string routingKey,
        IBasicProperties properties,
        ReadOnlyMemory<byte> body)
    {
        Guid guid = Guid.Parse(properties.CorrelationId);
        if ((properties.Headers?.ContainsKey("STREAM_END") ?? false) && properties.Headers["STREAM_END"] is true)
        {
            _responses.Remove(guid, out BufferBlock<TResponse>? responseBlock);
            responseBlock?.Complete();
        }
        else
        {
            BufferBlock<TResponse>? responseBlock;
            try
            {
                responseBlock = _responses[guid];
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            TResponse response = MessagePackSerializer.Deserialize<TResponse>(body);
            await responseBlock.SendAsync(response).ConfigureAwait(false);
        }
    }

    public async IAsyncEnumerable<TResponse> CallAsync(string queueName, TRequest arg, [EnumeratorCancellation] CancellationToken token = default)
    {
        Guid guid = Guid.NewGuid();
        Logger.LogInformation($"Init {guid}…");
        byte[] bytes = MessagePackSerializer.Serialize(arg);

        IBasicProperties properties = Model.CreateBasicProperties();
        properties.ReplyTo = ReplyQueueName;
        properties.CorrelationId = guid.ToString("N");
        properties.AppId = AppId;
        Logger.LogInformation($"End init {guid}");

        BufferBlock<TResponse> buffer = new();
        _responses.AddOrUpdate(guid, buffer, (_, _) => throw new DuplicateWaitObjectException(guid.ToString()));
        Logger.LogInformation($"Send {guid}");
        Model.BasicPublish(string.Empty, queueName, properties, bytes);
        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token, CancellationTokenSource.Token);
        cts.CancelAfter(30000);
        await foreach (TResponse response in buffer.ReceiveAllAsync(cts.Token).ConfigureAwait(false)) yield return response;
        Logger.LogInformation($"Finish {guid}");
    }
}