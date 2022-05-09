using System.Reflection;
using And9.Lib.Broker.Consumers;
using And9.Lib.Broker.ConsumerStrategy;
using And9.Lib.Broker.Publishers;
using And9.Lib.Broker.Senders;
using Microsoft.Extensions.DependencyInjection;

namespace And9.Lib.Broker;

public class BrokerBuilder
{
    private readonly Dictionary<string, Type> _consumers = new();
    private readonly Dictionary<string, Func<IServiceScope, BaseBrokerPublisher>> _publishers = new();

    internal BrokerBuilder(IServiceCollection services) => Services = services;

    internal IServiceCollection Services { get; }

    public BrokerBuilder AppendConsumerWithoutResponse<TStrategy, TArg>() where TStrategy : class, IBrokerConsumerWithoutResponseStrategy<TArg> =>
        AppendConsumerWithoutResponse<TStrategy, TArg>(typeof(TStrategy).GetCustomAttribute<QueueNameAttribute>()?.QueueName ?? throw new ArgumentNullException());

    public BrokerBuilder AppendConsumerWithoutResponse<TStrategy, TArg>(string queueName) where TStrategy : class, IBrokerConsumerWithoutResponseStrategy<TArg>
    {
        Services.AddScoped<TStrategy>();
        _consumers.Add(queueName, typeof(BrokerConsumerWithoutResponse<TArg, TStrategy>));
        Services.AddScoped<BrokerConsumerWithoutResponse<TArg, TStrategy>>();
        return this;
    }

    public BrokerBuilder AppendConsumerWithResponse<TStrategy, TArg, TResponse>() where TStrategy : class, IBrokerConsumerWithResponseStrategy<TArg, TResponse> =>
        AppendConsumerWithResponse<TStrategy, TArg, TResponse>(typeof(TStrategy).GetCustomAttribute<QueueNameAttribute>()?.QueueName ?? throw new ArgumentNullException());

    public BrokerBuilder AppendConsumerWithResponse<TStrategy, TArg, TResponse>(string queueName) where TStrategy : class, IBrokerConsumerWithResponseStrategy<TArg, TResponse>
    {
        Services.AddScoped<TStrategy>();
        _consumers.Add(queueName, typeof(BrokerConsumerWithResponse<TArg, TResponse, TStrategy>));
        Services.AddScoped<BrokerConsumerWithResponse<TArg, TResponse, TStrategy>>();
        return this;
    }

    public BrokerBuilder AppendConsumerWithCollectionResponse<TStrategy, TArg, TResponse>() where TStrategy : class, IBrokerConsumerWithCollectionResponseStrategy<TArg, TResponse> =>
        AppendConsumerWithCollectionResponse<TStrategy, TArg, TResponse>(typeof(TStrategy).GetCustomAttribute<QueueNameAttribute>()?.QueueName ?? throw new ArgumentNullException());

    public BrokerBuilder AppendConsumerWithCollectionResponse<TStrategy, TArg, TResponse>(string queueName) where TStrategy : class, IBrokerConsumerWithCollectionResponseStrategy<TArg, TResponse>
    {
        Services.AddScoped<TStrategy>();
        _consumers.Add(queueName, typeof(BrokerConsumerWithCollectionResponse<TArg, TResponse, TStrategy>));
        Services.AddScoped<BrokerConsumerWithCollectionResponse<TArg, TResponse, TStrategy>>();
        return this;
    }

    public BrokerBuilder AppendSenderWithoutResponse<TSender, TArg>() where TSender : BrokerSenderWithoutResponse<TArg> =>
        AppendSenderWithoutResponse<TSender, TArg>(typeof(TSender).GetCustomAttribute<QueueNameAttribute>()?.QueueName ?? throw new ArgumentNullException());

    public BrokerBuilder AppendSenderWithoutResponse<TSender, TArg>(string queueName) where TSender : BrokerSenderWithoutResponse<TArg>
    {
        Services.AddTransient<BrokerPublisherWithoutResponse<TArg>>();
        Services.AddScoped<TSender>();
        _publishers.Add(queueName, scope => scope.ServiceProvider.GetRequiredService<BrokerPublisherWithoutResponse<TArg>>());
        return this;
    }

    public BrokerBuilder AppendSenderWithResponse<TSender, TArg, TResponse>() where TSender : BrokerSenderWithResponse<TArg, TResponse> =>
        AppendSenderWithResponse<TSender, TArg, TResponse>(typeof(TSender).GetCustomAttribute<QueueNameAttribute>()?.QueueName ?? throw new ArgumentNullException());

    public BrokerBuilder AppendSenderWithResponse<TSender, TArg, TResponse>(string queueName) where TSender : BrokerSenderWithResponse<TArg, TResponse>
    {
        Services.AddTransient<BrokerPublisherWithResponse<TArg, TResponse>>();
        Services.AddScoped<TSender>();
        _publishers.Add(queueName, scope => scope.ServiceProvider.GetRequiredService<BrokerPublisherWithResponse<TArg, TResponse>>());
        return this;
    }

    public BrokerBuilder AppendSenderWithCollectionResponse<TSender, TArg, TResponse>() where TSender : BrokerSenderWithCollectionResponse<TArg, TResponse> =>
        AppendSenderWithCollectionResponse<TSender, TArg, TResponse>(typeof(TSender).GetCustomAttribute<QueueNameAttribute>()?.QueueName ?? throw new ArgumentNullException());

    public BrokerBuilder AppendSenderWithCollectionResponse<TSender, TArg, TResponse>(string queueName) where TSender : BrokerSenderWithCollectionResponse<TArg, TResponse>
    {
        Services.AddTransient<BrokerPublisherWithCollectionResponse<TArg, TResponse>>();
        Services.AddScoped<TSender>();
        _publishers.Add(queueName, scope => scope.ServiceProvider.GetRequiredService<BrokerPublisherWithCollectionResponse<TArg, TResponse>>());
        return this;
    }

    public IServiceCollection Build()
    {
        Services.AddSingleton<BrokerManager>(provider => new(provider.GetRequiredService<IServiceScopeFactory>())
        {
            Consumers = _consumers,
            Publishers = _publishers,
        });
        Services.AddHostedService(provider => provider.GetRequiredService<BrokerManager>());
        return Services;
    }
}