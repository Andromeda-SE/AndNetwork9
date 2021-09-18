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

        public async System.Threading.Tasks.Task CallAsync(TRequest arg)
        {
            Logger.LogDebug("Start call");
            Guid guid = Guid.NewGuid();
            Logger.LogInformation($"Init {guid}…");
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(arg, JsonSerializerOptions);

            IBasicProperties properties = Model.CreateBasicProperties();

            properties.ReplyTo = ReplyQueueName;
            properties.CorrelationId = guid.ToString("N");
            Logger.LogInformation($"End init {guid}");
            Waiting.AddOrUpdate(guid,
                _ => new(false),
                (_, oldEvent) =>
                {
                    oldEvent?.Dispose();
                    return new(false);
                });
            Model.BasicConsume(ReplyQueueName, false, Consumer);
            Model.BasicPublish(string.Empty, MethodQueueName, properties, bytes);
            Logger.LogInformation($"Send {guid}");
            await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    Logger.LogWarning($"Start wait {guid}");
                    if (!Waiting[guid].WaitOne(30000))
                    {
                        Logger.LogWarning($"Timeout {guid}");
                        throw new TimeoutException();
                    }
                    Logger.LogInformation($"End wait {guid}");
                    BasicDeliverEventArgs reply = Replies[guid];
                    Logger.LogDebug("Ack {0}", properties.CorrelationId);
                    Model.BasicAck(reply.DeliveryTag, false);
                    if (reply.BasicProperties.Headers["Success"] is true)
                    {
                        Logger.LogInformation($"{guid} OK");
                    }
                    else
                    {
                        Logger.LogWarning($"Error {guid}");
                        byte[]? exceptionData = reply.BasicProperties.Headers["Exception"] as byte[];
                        Exception? exception = JsonSerializer.Deserialize<Exception>(exceptionData);
                        throw exception ?? new Exception();
                    }
                }
                finally
                {
                    Logger.LogInformation($"End {guid}");
                    Waiting.TryRemove(guid, out ManualResetEvent? @event);
                    Replies.TryRemove(guid, out _);
                    @event?.Dispose();
                    Logger.LogInformation($"Finish {guid}");
                }
            }).ConfigureAwait(false);
        }

        protected override async void Received(object sender, BasicDeliverEventArgs args)
        {
            Logger.LogInformation($"Received {args.DeliveryTag}");
            await System.Threading.Tasks.Task.Run(() =>
            {
                Guid guid = Guid.Parse(args.BasicProperties.CorrelationId);
                Replies.AddOrUpdate(guid, _ => args, (_, _) => args);
                return Waiting[guid].Set();
            }).ConfigureAwait(false);
        }
    }
}