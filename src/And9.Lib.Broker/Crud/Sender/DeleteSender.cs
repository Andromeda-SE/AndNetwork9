using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Crud.Sender;

internal class DeleteSender : BaseRabbitSenderWithoutResponse<int>
{
    internal DeleteSender(IConnection connection, string queue, ILogger<BaseRabbitSenderWithoutResponse<int>> logger) : base(connection, queue + ".Delete", logger) { }
}