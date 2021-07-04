using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Storage.Listeners;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AndNetwork9.Storage
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ClanDataContext>(x =>
                x.UseNpgsql(configuration["Postgres:ConnectionString"]).UseLazyLoadingProxies());

            RabbitConnectionPool.SetConfiguration(configuration);
            services.AddScoped(_ => RabbitConnectionPool.Factory.CreateConnection());

            services.AddHostedService<RepoGetFileListener>();
            services.AddHostedService<RepoSetFileListener>();
            services.AddHostedService<NewRepoListener>();
        }
    }
}