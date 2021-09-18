using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AndNetwork9.Shared.Backend.Rabbit
{
    public abstract class BaseRabbitSender : IDisposable
    {
        protected static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            WriteIndented = false,
        };

        protected readonly EventingBasicConsumer Consumer;
        protected readonly string MethodQueueName;

        protected readonly IModel Model;
        protected readonly ConcurrentDictionary<Guid, BasicDeliverEventArgs> Replies = new();

        protected readonly string ReplyQueueName;

        protected readonly ConcurrentDictionary<Guid, ManualResetEvent> Waiting = new();
        protected readonly ILogger<BaseRabbitSender> Logger;

        protected BaseRabbitSender(IConnection connection, string queue, ILogger<BaseRabbitSender> logger)
        {
            Logger = logger;
            Logger.LogDebug("Creating…");
            Model = connection.CreateModel();
            MethodQueueName = queue;
            
            ReplyQueueName = Model.QueueDeclare().QueueName;
            Logger.LogDebug($"Reply queue name = {ReplyQueueName}");
            Consumer = new(Model);
            Consumer.Received += Received!;
            Logger.LogDebug("Sender created");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Model.Dispose();
        }

        protected abstract void Received(object sender, BasicDeliverEventArgs args);
    }
}