using And9.Lib.Models.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Crud.Sender;

internal class ReadSender<T> : BaseRabbitSenderWithResponse<int, T> where T : IId
{
    internal ReadSender(IConnection connection, string queue, ILogger<BaseRabbitSenderWithResponse<int, T>> logger) : base(connection, queue + ".Read", logger) { }
}