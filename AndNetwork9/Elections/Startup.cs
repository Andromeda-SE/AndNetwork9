using AndNetwork9.Elections.Listeners;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AndNetwork9.Elections;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEntityFrameworkProxies();
        services.AddDbContext<ClanDataContext>(x =>
            x.UseNpgsql(configuration["Postgres:ConnectionString"]).UseLazyLoadingProxies());

        RabbitConnectionPool.SetConfiguration(configuration);
        services.AddScoped(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddScoped<RewriteElectionsChannelSender>();

        services.AddHostedService<NextStage>();
        services.AddHostedService<Register>();
        services.AddHostedService<Vote>();
    }
}