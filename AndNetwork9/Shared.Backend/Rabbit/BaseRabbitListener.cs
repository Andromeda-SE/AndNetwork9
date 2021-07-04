using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AndNetwork9.Shared.Backend.Rabbit
{
    public abstract class BaseRabbitListener : IHostedService
    {
        protected static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            WriteIndented = false,
            IgnoreReadOnlyProperties = true,
        };

        protected readonly IConnection Connection;
        protected readonly string QueueName;
        protected EventingBasicConsumer Consumer = null!;

        protected IModel Model = null!;

        protected BaseRabbitListener(IConnection connection, string queueName)
        {
            Connection = connection;
            QueueName = queueName;
        }

        public System.Threading.Tasks.Task StartAsync(CancellationToken cancellationToken)
        {
            Model = Connection.CreateModel();

            Model.QueueDeclare(QueueName, false, false, false);
            Consumer = new(Model);
            Consumer.Received += Received!;
            Model.BasicConsume(QueueName, false, Consumer);
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
        {
            Consumer.Received -= Received!;
            Model.Dispose();
            return System.Threading.Tasks.Task.CompletedTask;
        }

        protected abstract void Received(object sender, BasicDeliverEventArgs args);
    }
}