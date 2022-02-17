using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace And9.Lib.Broker;

public abstract class BaseRabbitListener : IHostedService
{
    protected readonly IConnection Connection;
    protected static readonly object _sync = new();

    protected readonly ILogger<BaseRabbitListener> Logger;
    protected readonly string QueueName;
    protected EventingBasicConsumer Consumer = null!;

    protected IModel Model = null!;

    protected BaseRabbitListener(IConnection connection, string queueName, ILogger<BaseRabbitListener> logger)
    {
        Connection = connection;
        QueueName = queueName;
        Logger = logger;
    }

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogDebug("Starting…");
        Model = Connection.CreateModel();

        Model.QueueDeclare(QueueName, false, false, false);
        Consumer = new(Model);
        Consumer.Received += (sender, args) => Received(sender, args);
        Model.BasicConsume(QueueName, false, Consumer);
        Logger.LogDebug("Started");
        return Task.CompletedTask;
    }


    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogDebug("Stopping…");
        Consumer.Received -= (sender, args) => Received(sender, args);
        Model.Dispose();
        Logger.LogDebug("Stopped");
        return Task.CompletedTask;
    }

    protected abstract Task Received(object sender, BasicDeliverEventArgs args);
}