using System;
using System.Collections.Generic;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AndNetwork9.Shared.Backend.Rabbit
{
    public abstract class BaseRabbitListenerWithoutResponse<TRequest> : BaseRabbitListener
    {
        protected BaseRabbitListenerWithoutResponse(IConnection connection, string queue) : base(connection, queue) { }

        protected override void Received(object sender, BasicDeliverEventArgs args)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                IBasicProperties replyProperties = Model.CreateBasicProperties();
                replyProperties.Headers = new Dictionary<string, object>();
                replyProperties.CorrelationId = args.BasicProperties.CorrelationId;

                TRequest? request = JsonSerializer.Deserialize<TRequest>(args.Body.Span, JsonSerializerOptions);
                if (request is null) throw new ArgumentException();
                try
                {
                    Run(request);
                    replyProperties.Headers.Add("Success", true);
                }
                catch (Exception e)
                {
                    replyProperties.Headers.Add("Success", false);
                    replyProperties.Headers.Add("Exception",
                        JsonSerializer.SerializeToUtf8Bytes(e, JsonSerializerOptions));
                }
                finally
                {
                    Model.BasicPublish(string.Empty, args.BasicProperties.ReplyTo, replyProperties,
                        ReadOnlyMemory<byte>.Empty);
                    Model.BasicAck(args.DeliveryTag, false);
                }
            });
        }

        public abstract void Run(TRequest request);
    }
}