using System.Text.Json;
using MessagePack;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace And9.Lib.Broker;

public abstract class BaseRabbitListenerWithoutResponse<TRequest> : BaseRabbitListener
{
    protected BaseRabbitListenerWithoutResponse(IConnection connection, string queue,
        ILogger<BaseRabbitListenerWithoutResponse<TRequest>> logger) : base(connection, queue, logger) { }

    protected override Task Received(object sender, BasicDeliverEventArgs args)
    {
        try
        {
            Logger.LogInformation(
                $"Received {args.DeliveryTag}, CorrelationId = {args.BasicProperties.CorrelationId}");
            Logger.LogInformation($"Ack {args.DeliveryTag}");
            lock (_sync)
            {
                Model.BasicAck(args.DeliveryTag, false);
            }

            IBasicProperties replyProperties;
            lock (_sync)
            {
                replyProperties = Model.CreateBasicProperties();
            }

            replyProperties.Headers = new Dictionary<string, object>();
            replyProperties.CorrelationId = args.BasicProperties.CorrelationId;
            TRequest? request = MessagePackSerializer.Deserialize<TRequest>(args.Body);
            if (request is null) throw new ArgumentException();
            /*ValidationContext validationContext = new ValidationContext(request);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, validationContext, validationResults))
            {
                throw new ArgumentOutOfRangeException(string.Join(Environment.NewLine, validationResults.Select(x => x.ErrorMessage)));
            }*/
            try
            {
                Logger.LogInformation($"Run {args.DeliveryTag}");
                Task t = Run(request);
                t.Wait();
                Logger.LogInformation($"End run {args.DeliveryTag}");
                replyProperties.Headers.Add("Success", true);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, "Exception {0}", args.DeliveryTag);
                replyProperties.Headers.Add("Success", false);
                replyProperties.Headers.Add("Exception",
                    MessagePackSerializer.Serialize(
                        $"{e}{Environment.NewLine}{e.Message}{Environment.NewLine}{e.StackTrace}"));
            }
            finally
            {
                if (args.BasicProperties.ReplyTo is not null)
                {
                    Logger.LogInformation($"Publish {args.DeliveryTag}");
                    ReadOnlyMemory<byte> result = ReadOnlyMemory<byte>.Empty;
                    lock (_sync)
                    {
                        Model.BasicPublish(string.Empty, args.BasicProperties.ReplyTo, replyProperties, result);
                    }
                }

                Logger.LogInformation($"Finish {args.DeliveryTag}");
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"Unhandled exception {args.DeliveryTag}");
        }

        return Task.CompletedTask;
    }

    public abstract Task Run(TRequest request);
}