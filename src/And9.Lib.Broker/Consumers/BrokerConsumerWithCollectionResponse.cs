using System.Runtime.CompilerServices;
using And9.Lib.Broker.ConsumerStrategy;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Consumers;

public class BrokerConsumerWithCollectionResponse<TRequest, TResponse, TStrategy> : AsyncDefaultBasicConsumer where TStrategy : IBrokerConsumerWithCollectionResponseStrategy<TRequest, TResponse>
{
    private readonly ILogger<BrokerConsumerWithCollectionResponse<TRequest, TResponse, TStrategy>> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BrokerConsumerWithCollectionResponse(IServiceScopeFactory serviceScopeFactory, ILogger<BrokerConsumerWithCollectionResponse<TRequest, TResponse, TStrategy>> logger, IModel model)
        : base(model)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public override async Task HandleBasicDeliver(
        string consumerTag,
        ulong deliveryTag,
        bool redelivered,
        string exchange,
        string routingKey,
        IBasicProperties properties,
        ReadOnlyMemory<byte> body)
    {
        _logger.LogInformation($"Received {deliveryTag}, CorrelationId = {properties.CorrelationId}");
        Model.BasicAck(deliveryTag, false);
        IBasicProperties? replyProperties = Model.CreateBasicProperties();
        replyProperties.Headers = new Dictionary<string, object>();
        replyProperties.CorrelationId = properties.CorrelationId;
        TRequest? request = MessagePackSerializer.Deserialize<TRequest>(body);
        _logger.LogInformation($"Run {deliveryTag}");
        replyProperties.Headers.Add("Success", true);
        replyProperties.Headers.Add("STREAM_END", false);
        try
        {
            AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
            IAsyncEnumerable<TResponse?> response = scope.ServiceProvider.GetRequiredService<TStrategy>().ExecuteAsync(request);
            await foreach (TResponse? item in response.ConfigureAwait(false)) Model.BasicPublish(string.Empty, properties.ReplyTo, replyProperties, MessagePackSerializer.Serialize(item));
            _logger.LogInformation($"End run {deliveryTag}");
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Exception {0}", deliveryTag);
            replyProperties.Headers["Success"] = false;
            replyProperties.Headers.Add("Exception",
                MessagePackSerializer.Serialize($"{e}{Environment.NewLine}{e.Message}{Environment.NewLine}{e.StackTrace}"));
        }
        finally
        {
            replyProperties.Headers["STREAM_END"] = true;
            if (properties.ReplyTo is not null)
            {
                _logger.LogInformation($"Publish {deliveryTag}");
                Model.BasicPublish(string.Empty, properties.ReplyTo, replyProperties, ReadOnlyMemory<byte>.Empty);
            }

            _logger.LogInformation($"Finish {deliveryTag}");
        }
    }
}