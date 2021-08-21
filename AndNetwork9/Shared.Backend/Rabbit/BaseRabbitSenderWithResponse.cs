using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AndNetwork9.Shared.Backend.Rabbit
{
    public class BaseRabbitSenderWithResponse<TRequest, TResponse> : BaseRabbitSender
    {
        protected BaseRabbitSenderWithResponse(IConnection connection, string queue) : base(connection, queue) { }

        public async Task<TResponse?> CallAsync(TRequest arg)
        {
            Guid guid = Guid.NewGuid();
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(arg, JsonSerializerOptions);

            IBasicProperties properties = Model.CreateBasicProperties();
            properties.ReplyTo = ReplyQueueName;
            properties.CorrelationId = guid.ToString("N");
            Waiting.AddOrUpdate(guid,
                _ => new(false),
                (_, oldEvent) =>
                {
                    oldEvent?.Dispose();
                    return new(false);
                });
            Model.BasicConsume(ReplyQueueName, false, Consumer);
            Model.BasicPublish(string.Empty, MethodQueueName, properties, bytes);
            return await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    if (!Waiting[guid].WaitOne(30000)) throw new TimeoutException();

                    BasicDeliverEventArgs reply = Replies[guid];
                    Model.BasicAck(reply.DeliveryTag, false);
                    if (reply.BasicProperties.Headers["Success"] is true)
                    {
                        return JsonSerializer.Deserialize<TResponse>(reply.Body.Span, JsonSerializerOptions);
                    }
                    else
                    {
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
                }
            }).ConfigureAwait(false);
        }

        protected override async void Received(object sender, BasicDeliverEventArgs args)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                Guid guid = Guid.Parse(args.BasicProperties.CorrelationId);
                Replies.AddOrUpdate(guid, _ => args, (_, _) => args);
                return Waiting[guid].Set();
            }).ConfigureAwait(false);
        }
    }
}