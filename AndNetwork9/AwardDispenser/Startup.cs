using AndNetwork9.AwardDispenser.Listeners;
using AndNetwork9.AwardDispenser.Services;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.AwardDispenser;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Backend.Senders.Steam;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AndNetwork9.AwardDispenser
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ClanDataContext>(x =>
                x.UseNpgsql(configuration["Postgres:ConnectionString"])
                    .UseLazyLoadingProxies());

            RabbitConnectionPool.SetConfiguration(configuration);
            services.AddScoped(_ => RabbitConnectionPool.Factory.CreateConnection());

            services.AddSingleton<GiveAwardSender>();
            services.AddSingleton<PublishSender>();
            services.AddSingleton<PlayerActivitySender>();

            services.AddHostedService<Services.AwardDispenser>();
            services.AddHostedService<RiseDispenser>();

            services.AddHostedService<GiveAward>();
        }
    }
}