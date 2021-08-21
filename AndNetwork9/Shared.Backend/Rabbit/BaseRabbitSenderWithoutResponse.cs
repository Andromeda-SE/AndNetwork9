using System;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AndNetwork9.Shared.Backend.Rabbit
{
    public class BaseRabbitSenderWithoutResponse<TRequest> : BaseRabbitSender
    {
        protected BaseRabbitSenderWithoutResponse(IConnection connection, string queue, ILogger<BaseRabbitSenderWithoutResponse<TRequest>> logger) : base(connection, queue, logger) { }

        public System.Threading.Tasks.Task CallAsync(TRequest arg)
        {
            Logger.LogDebug("Start call");
            Guid guid = Guid.NewGuid();
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(arg, JsonSerializerOptions);

            IBasicProperties properties = Model.CreateBasicProperties();

            properties.ReplyTo = ReplyQueueName;
            properties.CorrelationId = guid.ToString("N");
            Logger.LogInformation("Called: ReplyTo = «{0}»; CorrelationId = «{1}»", properties.ReplyTo, properties.CorrelationId);
            Waiting.AddOrUpdate(guid,
                _ => new(false),
                (_, oldEvent) =>
                {
                    oldEvent?.Dispose();
                    return new(false);
                });
            Logger.LogDebug("Publish {0}", properties.CorrelationId);
            Model.BasicPublish(string.Empty, MethodQueueName, properties, bytes);
            Model.BasicConsume(ReplyQueueName, false, Consumer);
            return System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    Logger.LogDebug("Start waiting {0}", properties.CorrelationId);
                    if (!Waiting[guid].WaitOne(30000)) throw new TimeoutException();

                    BasicDeliverEventArgs reply = Replies[guid];
                    Logger.LogDebug("Ack {0}", properties.CorrelationId);
                    Model.BasicAck(reply.DeliveryTag, false);
                    if (reply.BasicProperties.Headers["Success"] is true)
                    {
                        Logger.LogInformation("OK {0}", properties.CorrelationId);
                    }
                    else
                    {
                        Logger.LogInformation("Exception {0}", properties.CorrelationId);
                        byte[]? exceptionData = reply.BasicProperties.Headers["Exception"] as byte[];
                        Exception? exception = JsonSerializer.Deserialize<Exception>(exceptionData);
                        throw exception ?? new Exception();
                    }
                }
                finally
                {
                    Waiting.TryRemove(guid, out ManualResetEvent? @event);
                    Replies.TryRemove(guid, out _);
                    @event?.Dispose();
                    Logger.LogInformation("Final {0}", properties.CorrelationId);
                }
            });
        }

        protected override void Received(object sender, BasicDeliverEventArgs args)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                Guid guid = Guid.Parse(args.BasicProperties.CorrelationId);
                Replies.AddOrUpdate(guid, _ => args, (_, _) => args);
                return Waiting[guid].Set();
            });
        }
    }
}