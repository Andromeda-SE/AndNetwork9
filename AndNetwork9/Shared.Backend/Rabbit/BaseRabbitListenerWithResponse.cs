using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AndNetwork9.Shared.Backend.Rabbit
{
    public abstract class BaseRabbitListenerWithResponse<TRequest, TResponse> : BaseRabbitListener
    {
        protected BaseRabbitListenerWithResponse(IConnection connection, string queue) : base(connection, queue) { }

        protected override async void Received(object sender, BasicDeliverEventArgs args)
        {
            await System.Threading.Tasks.Task.Run(async () =>
            {
                IBasicProperties replyProperties = Model.CreateBasicProperties();
                replyProperties.Headers = new Dictionary<string, object>();
                replyProperties.CorrelationId = args.BasicProperties.CorrelationId;

                TRequest? request = JsonSerializer.Deserialize<TRequest>(args.Body.Span, JsonSerializerOptions);
                TResponse? response = default;
                try
                {
                    if (request is null) throw new ArgumentException("request is null", nameof(args));
                    response = await GetResponseAsync(request).ConfigureAwait(false);
                    replyProperties.Headers.Add("Success", true);
                }
                catch (Exception e)
                {
                    replyProperties.Headers.Add("Success", false);
                    replyProperties.Headers.Add("Exception", JsonSerializer.Serialize(e, JsonSerializerOptions));
                }
                finally
                {
                    ReadOnlyMemory<byte> result = response is null
                        ? ReadOnlyMemory<byte>.Empty
                        : JsonSerializer.SerializeToUtf8Bytes(response, JsonSerializerOptions);
                    Model.BasicPublish(string.Empty, args.BasicProperties.ReplyTo, replyProperties, result);
                    Model.BasicAck(args.DeliveryTag, false);
                }
            }).ConfigureAwait(false);
        }

        protected abstract Task<TResponse> GetResponseAsync(TRequest request);
    }
}