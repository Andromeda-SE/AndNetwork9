using System.Net.Http;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Steam.Listeners;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AndNetwork9.Steam;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        RabbitConnectionPool.SetConfiguration(configuration);
        services.AddScoped(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddScoped<HttpClient>();

        services.AddHostedService<PlayerActivity>();
        services.AddHostedService<ResolveSteamUrl>();
    }
}