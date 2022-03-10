using System.Collections.Concurrent;
using MessagePack;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace And9.Lib.Broker;

public class BaseRabbitSenderWithStreamResponse<TRequest, TResponse> : BaseRabbitSender
{
    protected readonly ConcurrentDictionary<Guid, ConcurrentQueue<BasicDeliverEventArgs?>> StreamReplies = new();
    public BaseRabbitSenderWithStreamResponse(IConnection connection, string queue, ILogger<BaseRabbitSender> logger) : base(connection, queue, logger) { }

    public async IAsyncEnumerable<TResponse> CallAsync(TRequest arg)
    {
        Guid guid = Guid.NewGuid();
        Logger.LogInformation($"Init {guid}…");
        byte[] bytes = MessagePackSerializer.Serialize(arg);
        IBasicProperties properties;
        lock (Model)
        {
            properties = Model.CreateBasicProperties();
        }

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
        lock (Model)
        {
            Model.BasicConsume(ReplyQueueName, false, Consumer);
        }

        Logger.LogInformation($"Send {guid}");
        lock (Model)
        {
            Model.BasicPublish(string.Empty, MethodQueueName, properties, bytes);
        }

        foreach (TResponse response in AsyncEnumerable(guid)) yield return response;
    }

    private IEnumerable<TResponse?> AsyncEnumerable(Guid guid)
    {
        try
        {
            while (true)
            {
                if (!Waiting[guid].WaitOne(30000))
                {
                    Logger.LogWarning($"Timeout {guid}");
                    throw new TimeoutException();
                }

                if (StreamReplies[guid].TryDequeue(out BasicDeliverEventArgs? reply))
                {
                    if (reply is null) throw new();
                    lock (Model)
                    {
                        Model.BasicAck(reply.DeliveryTag, false);
                    }

                    if ((reply.BasicProperties.Headers?.ContainsKey("STREAM_END") ?? false)
                        && reply.BasicProperties.Headers["STREAM_END"] is true) yield break;
                    if (reply.BasicProperties.Headers["Success"] is false)
                    {
                        Logger.LogWarning($"Error {guid}");
                        byte[]? exceptionData = reply.BasicProperties.Headers["Exception"] as byte[];
                        string? exception = MessagePackSerializer.Deserialize<string>(exceptionData);
                        throw new(exception);
                    }

                    TResponse response;
                    try
                    {
                        response = reply.Body.IsEmpty
                            ? default!
                            : MessagePackSerializer.Deserialize<TResponse>(reply.Body);
                    }
                    catch (Exception e)
                    {
                        string text = MessagePackSerializer.ConvertToJson(reply.Body);
                        Logger.LogError(e, "Exception on serializing. Source text: {0}", text);
                        throw;
                    }

                    yield return response;
                    Logger.LogInformation($"{guid} OK");
                }

                Logger.LogInformation($"End wait {guid}");
            }
        }
        finally
        {
            Logger.LogInformation($"End {guid}");
            Waiting.TryRemove(guid, out ManualResetEvent? @event);
            StreamReplies.TryRemove(guid, out _);
            @event?.Dispose();
            Logger.LogInformation($"Finish {guid}");
        }
    }

    protected override void Received(object sender, BasicDeliverEventArgs args)
    {
        Logger.LogInformation($"Received {args.BasicProperties.CorrelationId}");
        Guid guid = Guid.Parse(args.BasicProperties.CorrelationId);
        ConcurrentQueue<BasicDeliverEventArgs?> queue = StreamReplies.GetOrAdd(guid, new ConcurrentQueue<BasicDeliverEventArgs?>());
        queue.Enqueue(args);
        bool result = Waiting[guid].Set();
        if (!result) throw new("result is false");
    }
}