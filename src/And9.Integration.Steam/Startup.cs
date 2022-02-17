using And9.Integration.Steam.Listeners;
using And9.Lib.Broker;

namespace And9.Integration.Steam;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        RabbitConnectionPool.SetConfiguration(configuration);
        services.AddSingleton(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddScoped<HttpClient>();

        services.AddHostedService<PlayerActivity>();
        services.AddHostedService<ResolveSteamUrl>();
    }
}