using And9.Lib.Models.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Lib.Broker.Crud.Sender;

internal class CreateSender<T> : BaseRabbitSenderWithResponse<T, int> where T : IId
{
    internal CreateSender(IConnection connection, string queue, ILogger<BaseRabbitSenderWithResponse<T, int>> logger) : base(connection, queue + ".Create", logger) { }
}