using System.Runtime.CompilerServices;
using And9.Lib.Broker.ConsumerStrategy;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace And9.Lib.Broker.Consumers;

public class BrokerConsumerWithResponse<TRequest, TResponse, TStrategy> : AsyncDefaultBasicConsumer where TStrategy : IBrokerConsumerWithResponseStrategy<TRequest, TResponse>
{
    private readonly ILogger<BrokerConsumerWithResponse<TRequest, TResponse, TStrategy>> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BrokerConsumerWithResponse(IServiceScopeFactory serviceScopeFactory, ILogger<BrokerConsumerWithResponse<TRequest, TResponse, TStrategy>> logger, IModel model) : base(model)
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
        //await base.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body).ConfigureAwait(false);
        Model.BasicAck(deliveryTag, false);
        _logger.LogInformation($"Received {deliveryTag}, CorrelationId = {properties.CorrelationId}");
        IBasicProperties? replyProperties = Model.CreateBasicProperties();
        replyProperties.Headers = new Dictionary<string, object>();
        replyProperties.CorrelationId = properties.CorrelationId;
        TRequest? request = body.Length == 0 ? default : MessagePackSerializer.Deserialize<TRequest>(body);
        _logger.LogInformation($"Run {deliveryTag}");
        TResponse? response = default;
        try
        {
            AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
            response = await scope.ServiceProvider.GetRequiredService<TStrategy>().ExecuteAsync(request).ConfigureAwait(false);
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
                Model.BasicPublish(string.Empty,
                    properties.ReplyTo,
                    replyProperties,
                    response is null
                        ? ReadOnlyMemory<byte>.Empty
                        : MessagePackSerializer.Serialize(response));
            }

            _logger.LogInformation($"Finish {deliveryTag}");
        }
    }
}