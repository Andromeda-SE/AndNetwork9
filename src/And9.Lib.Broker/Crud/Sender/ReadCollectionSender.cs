using And9.Lib.Models.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Crud.Sender;

public class ReadCollectionSender<T> : BaseRabbitSenderWithStreamResponse<object, T> where T : IId
{
    public ReadCollectionSender(
        IConnection connection,
        string queue,
        string queueSuffix,
        ILogger<BaseRabbitSenderWithStreamResponse<object, T>> logger)
        : base(connection, queue + "." + queueSuffix, logger) { }
}