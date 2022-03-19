using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace And9.Lib.Broker;

public abstract class BaseRabbitSender : IDisposable
{
    private protected static readonly string AppId = Assembly.GetEntryAssembly()?.ToString() ?? string.Empty;

    protected readonly EventingBasicConsumer Consumer;
    protected readonly ILogger<BaseRabbitSender> Logger;
    protected readonly string MethodQueueName;

    protected readonly IModel Model;
    protected readonly ConcurrentDictionary<Guid, BasicDeliverEventArgs> Replies = new();

    protected readonly string ReplyQueueName;

    protected readonly ConcurrentDictionary<Guid, ManualResetEvent> Waiting = new();

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
        Consumer.Received -= Received!;
        Model.Dispose();
    }

    protected abstract void Received(object sender, BasicDeliverEventArgs args);
}