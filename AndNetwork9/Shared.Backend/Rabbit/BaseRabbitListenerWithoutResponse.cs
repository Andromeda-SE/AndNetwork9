using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AndNetwork9.Shared.Backend.Rabbit
{
    public abstract class BaseRabbitListenerWithoutResponse<TRequest> : BaseRabbitListener
    {
        protected BaseRabbitListenerWithoutResponse(IConnection connection, string queue, ILogger<BaseRabbitListenerWithoutResponse<TRequest>> logger) : base(connection, queue, logger) { }

        protected override void Received(object sender, BasicDeliverEventArgs args)
        {
            System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    Logger.LogInformation($"Received {args.DeliveryTag}");
                    IBasicProperties replyProperties = Model.CreateBasicProperties();
                    replyProperties.Headers = new Dictionary<string, object>();
                    replyProperties.CorrelationId = args.BasicProperties.CorrelationId;

                    TRequest? request = JsonSerializer.Deserialize<TRequest>(args.Body.Span, JsonSerializerOptions);
                    if (request is null) throw new ArgumentException();
                    try
                    {
                        Logger.LogInformation($"Run {args.DeliveryTag}");
                        await Run(request).ConfigureAwait(true);
                        Logger.LogInformation($"End run {args.DeliveryTag}");
                        replyProperties.Headers.Add("Success", true);
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarning(e, "Exception {0}", args.DeliveryTag);
                        replyProperties.Headers.Add("Success", false);
                        replyProperties.Headers.Add("Exception",
                            JsonSerializer.SerializeToUtf8Bytes(e, JsonSerializerOptions));
                    }
                    finally
                    {
                        Logger.LogInformation($"Ack {args.DeliveryTag}");
                        Model.BasicAck(args.DeliveryTag, false);
                        Logger.LogInformation($"Finish {args.DeliveryTag}");
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Unhandled exception {args.DeliveryTag}");
                }
            });
        }

        public abstract System.Threading.Tasks.Task Run(TRequest request);
    }
}