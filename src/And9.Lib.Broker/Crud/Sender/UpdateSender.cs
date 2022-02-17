using And9.Lib.Models.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Crud.Sender;

internal class UpdateSender<T> : BaseRabbitSenderWithResponse<T, T> where T : IId
{
    internal UpdateSender(IConnection connection, string queue, ILogger<BaseRabbitSenderWithResponse<T, T>> logger) : base(connection, queue + ".Update", logger) { }
}