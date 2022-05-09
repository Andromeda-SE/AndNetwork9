using System.Runtime.CompilerServices;
using And9.Lib.Broker.ConsumerStrategy;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Consumers;

public class BrokerConsumerWithoutResponse<TRequest, TStrategy> : AsyncDefaultBasicConsumer where TStrategy : IBrokerConsumerWithoutResponseStrategy<TRequest>
{
    private readonly ILogger<BrokerConsumerWithoutResponse<TRequest, TStrategy>> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BrokerConsumerWithoutResponse(IServiceScopeFactory serviceScopeFactory, ILogger<BrokerConsumerWithoutResponse<TRequest, TStrategy>> logger, IModel model) : base(model)
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
        try
        {
            AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
            await scope.ServiceProvider.GetRequiredService<TStrategy>().ExecuteAsync(request).ConfigureAwait(false);
            _logger.LogInformation($"End run {deliveryTag}");
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Exception {0}", deliveryTag);
            replyProperties.Headers.Add("Success", false);
            replyProperties.Headers.Add("Exception",
                MessagePackSerializer.Serialize($"{e}{Environment.NewLine}{e.Message}{Environment.NewLine}{e.StackTrace}"));
        }
        finally
        {
            if (properties.ReplyTo is not null)
            {
                _logger.LogInformation($"Publish {deliveryTag}");
                Model.BasicPublish(string.Empty, properties.ReplyTo, replyProperties, ReadOnlyMemory<byte>.Empty);
            }

            _logger.LogInformation($"Finish {deliveryTag}");
        }
    }
}