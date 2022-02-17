﻿using System.Text.Json;
using MessagePack;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace And9.Lib.Broker;

public class BaseRabbitSenderWithoutResponse<TRequest> : BaseRabbitSender
{
    protected BaseRabbitSenderWithoutResponse(IConnection connection, string queue,
        ILogger<BaseRabbitSenderWithoutResponse<TRequest>> logger) : base(connection, queue, logger) { }

    public Task CallAsync(TRequest arg)
    {
        Logger.LogDebug("Start call");
        Guid guid = Guid.NewGuid();
        Logger.LogInformation($"Init {guid}…");

        byte[] bytes = MessagePackSerializer.Serialize(arg);

        IBasicProperties properties = Model.CreateBasicProperties();

        properties.ReplyTo = ReplyQueueName;
        properties.CorrelationId = guid.ToString("N");
        properties.AppId = AppId;
        Logger.LogInformation($"End init {guid}");
        Waiting.AddOrUpdate(guid,
            _ => new(false),
            (_, oldEvent) =>
            {
                oldEvent?.Dispose();
                return new(false);
            });
        Model.BasicConsume(ReplyQueueName, false, Consumer);
        Logger.LogInformation($"Send {guid}");
        Model.BasicPublish(string.Empty, MethodQueueName, properties, bytes);
        return Task.Run(Wait);

        void Wait()
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
                    string? exception = MessagePackSerializer.Deserialize<string>(exceptionData);
                    throw new ArgumentException(exception);
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
        }
    }

    protected override void Received(object sender, BasicDeliverEventArgs args)
    {
        Logger.LogInformation($"Received {args.DeliveryTag}");
        Guid guid = Guid.Parse(args.BasicProperties.CorrelationId);
        Replies.AddOrUpdate(guid, _ => args, (_, _) => args);
        Waiting[guid].Set();
    }
}