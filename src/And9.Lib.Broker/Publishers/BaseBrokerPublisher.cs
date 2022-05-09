using System.Reflection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Publishers;

internal abstract class BaseBrokerPublisher : AsyncDefaultBasicConsumer, IDisposable
{
    private protected static readonly string AppId = Assembly.GetEntryAssembly()?.ToString() ?? string.Empty;

    private protected readonly CancellationTokenSource CancellationTokenSource = new();

    protected readonly ILogger<BaseBrokerPublisher> Logger;
    protected readonly string ReplyQueueName;

    private protected BaseBrokerPublisher(IModel model, ILogger<BaseBrokerPublisher> logger) : base(model)
    {
        Logger = logger;
        ReplyQueueName = Model.QueueDeclare(exclusive: false).QueueName;
        Model.BasicConsume(ReplyQueueName, false, this);
        Logger.LogInformation($"Reply queue name = {ReplyQueueName}");
    }

    public void Dispose()
    {
        Model.QueueDeleteNoWait(ReplyQueueName);
        CancellationTokenSource.Cancel();
        GC.SuppressFinalize(this);
    }

    public abstract override Task HandleBasicDeliver(
        string consumerTag,
        ulong deliveryTag,
        bool redelivered,
        string exchange,
        string routingKey,
        IBasicProperties properties,
        ReadOnlyMemory<byte> body);
}