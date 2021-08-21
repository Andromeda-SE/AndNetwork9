using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AndNetwork9.Shared.Backend.Rabbit
{
    public abstract class BaseRabbitListenerWithResponse<TRequest, TResponse> : BaseRabbitListener
    {
        protected BaseRabbitListenerWithResponse(IConnection connection, string queue, ILogger<BaseRabbitListenerWithResponse<TRequest, TResponse>> logger) : base(connection, queue, logger) { }

        protected override async void Received(object sender, BasicDeliverEventArgs args)
        {
            await System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    Logger.LogInformation($"Received {args.DeliveryTag}");
                    IBasicProperties replyProperties = Model.CreateBasicProperties();
                    replyProperties.Headers = new Dictionary<string, object>();
                    replyProperties.CorrelationId = args.BasicProperties.CorrelationId;

                    TRequest? request = JsonSerializer.Deserialize<TRequest>(args.Body.Span, JsonSerializerOptions);
                    if (request is null) throw new ArgumentException("request is null", nameof(args));
                    TResponse? response = default;
                    try
                    {
                        Logger.LogInformation($"Run {args.DeliveryTag}");
                        response = await GetResponseAsync(request).ConfigureAwait(true);
                        Logger.LogInformation($"End run {args.DeliveryTag}");
                        replyProperties.Headers.Add("Success", true);
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarning(e, "Exception {0}", args.DeliveryTag);
                        replyProperties.Headers.Add("Success", false);
                        replyProperties.Headers.Add("Exception", JsonSerializer.Serialize(e, JsonSerializerOptions));
                    }
                    finally
                    {
                        if (args.BasicProperties.ReplyTo is not null)
                        {
                            Logger.LogInformation($"Publish {args.DeliveryTag}");
                            ReadOnlyMemory<byte> result = response is null
                                ? ReadOnlyMemory<byte>.Empty
                                : JsonSerializer.SerializeToUtf8Bytes(response, JsonSerializerOptions);
                            Model.BasicPublish(string.Empty, args.BasicProperties.ReplyTo, replyProperties, result);
                        }
                        Logger.LogInformation($"Ack {args.DeliveryTag}");
                        Model.BasicAck(args.DeliveryTag, false);
                        Logger.LogInformation($"Finish {args.DeliveryTag}");
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Unhandled exception {args.DeliveryTag}");
                }

            }).ConfigureAwait(false);
        }

        protected abstract Task<TResponse> GetResponseAsync(TRequest request);
    }
}