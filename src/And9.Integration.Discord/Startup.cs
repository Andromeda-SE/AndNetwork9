using And9.Integration.Discord.Database;
using And9.Integration.Discord.Listeners;
using And9.Lib.Broker;
using Microsoft.EntityFrameworkCore;

namespace And9.Integration.Discord;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        RabbitConnectionPool.SetConfiguration(configuration);
        services.AddScoped(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddDbContext<DiscordDataContext>(x => x.UseNpgsql(configuration["Postgres:ConnectionString"]));

        services.AddSingleton<DiscordBot>();
        services.AddHostedService(provider => (DiscordBot)provider.GetService(typeof(DiscordBot))!);

        services.AddHostedService<SendDirectMessageListener>();
    }
}