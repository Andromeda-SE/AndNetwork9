using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace And9.Lib.Broker;

public static class BrokerExtensions
{
    public static BrokerBuilder WithBroker(this IServiceCollection services, IConfiguration configuration)
    {
        BrokerConnectionPool.SetConfiguration(configuration);
        services.AddSingleton<IConnectionFactory>(_ => BrokerConnectionPool.Factory);
        services.AddSingleton(_ => BrokerConnectionPool.Factory.CreateConnection());
        services.AddScoped(provider => provider.GetRequiredService<IConnection>().CreateModel());
        return new(services);
    }
}