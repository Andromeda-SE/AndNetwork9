using System.Text;
using System.Text.Json;
using And9.Lib.Models.Abstractions.Interfaces;
using MessagePack;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace And9.Lib.Broker;

public abstract class BaseRabbitListenerWithStreamResponse<TRequest, TResponse> : BaseRabbitListener where TResponse : IId
{
    public BaseRabbitListenerWithStreamResponse(IConnection connection, string queueName, ILogger<BaseRabbitListener> logger) : base(connection, queueName, logger) { }

    protected override async Task Received(object sender, BasicDeliverEventArgs args)
    {
        try
        {
            Logger.LogInformation($"Received {args.DeliveryTag}");
            Logger.LogInformation($"Ack {args.DeliveryTag}");
            lock (_sync)
            {
                Model.BasicAck(args.DeliveryTag, false);
            }

            IBasicProperties replyProperties;
            lock (_sync)
            {
                replyProperties = Model.CreateBasicProperties();
            }

            replyProperties.Headers = new Dictionary<string, object>();
            replyProperties.CorrelationId = args.BasicProperties.CorrelationId;
            TRequest? request = MessagePackSerializer.Deserialize<TRequest>(args.Body);
            if (request is null) throw new ArgumentException("request is null", nameof(args));
            IAsyncEnumerable<TResponse>? response = null;
            try
            {
                Logger.LogInformation($"Run {args.DeliveryTag}");
                response = GetResponseAsync(request);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, "Exception {0}", args.DeliveryTag);
                replyProperties.Headers.Add("Success", false);
                replyProperties.Headers.Add("Exception",
                    MessagePackSerializer.Serialize(
                        $"{e}{Environment.NewLine}{e.Message}{Environment.NewLine}{e.StackTrace}"));
            }

            if (args.BasicProperties.ReplyTo is not null && response is not null)
            {
                replyProperties.Headers.Add("Success", true);
                replyProperties.Headers.Add("STREAM_END", false);
                await foreach (byte[] bytes in response.Select(x => MessagePackSerializer.Serialize(x)).ConfigureAwait(false))
                    try
                    {
                        lock (_sync)
                        {
                            Model.BasicPublish(string.Empty, args.BasicProperties.ReplyTo, replyProperties, bytes);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogCritical(e, Encoding.UTF8.GetString(bytes));
                        throw;
                    }

                replyProperties.Headers["STREAM_END"] = true;
                lock (_sync)
                {
                    Model.BasicPublish(string.Empty, args.BasicProperties.ReplyTo, replyProperties, ReadOnlyMemory<byte>.Empty);
                }

                Logger.LogInformation($"Finish {args.DeliveryTag}");
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"Unhandled exception {args.DeliveryTag}");
        }
    }

    protected abstract IAsyncEnumerable<TResponse> GetResponseAsync(TRequest request, CancellationToken cancellationToken = default);
}