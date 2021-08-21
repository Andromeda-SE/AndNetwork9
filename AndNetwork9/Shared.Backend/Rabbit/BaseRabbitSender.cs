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

        protected readonly ILogger<BaseRabbitSender> Logger;
        protected readonly EventingBasicConsumer Consumer;
        protected readonly string MethodQueueName;

        protected readonly IModel Model;
        protected readonly ConcurrentDictionary<Guid, BasicDeliverEventArgs> Replies = new();

        protected readonly string ReplyQueueName;

        protected readonly ConcurrentDictionary<Guid, ManualResetEvent> Waiting = new();

        protected BaseRabbitSender(IConnection connection, string queue, ILogger<BaseRabbitSender> logger)
        {
            Logger = logger;

            Model = connection.CreateModel();
            MethodQueueName = queue;
            Logger.LogDebug("Init started…");
            ReplyQueueName = Model.QueueDeclare().QueueName;

            Consumer = new(Model);
            Consumer.Received += Received!;
            Logger.LogDebug("Init end. ReplyQueueName == «{0}»", ReplyQueueName);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Model.Dispose();
        }

        protected abstract void Received(object sender, BasicDeliverEventArgs args);
    }
}