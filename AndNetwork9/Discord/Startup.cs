using AndNetwork9.Discord.Listeners;
using AndNetwork9.Discord.Services;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Backend.Senders.Elections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AndNetwork9.Discord;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ClanDataContext>(x =>
            x.UseNpgsql(configuration["Postgres:ConnectionString"])
                .UseLazyLoadingProxies());

        RabbitConnectionPool.SetConfiguration(configuration);
        services.AddScoped(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddSingleton<NextStageSender>();
        services.AddSingleton<RegisterSender>();
        services.AddSingleton<PublishSender>();
        services.AddSingleton<UpdateUserSender>();

        services.AddSingleton<RoleManager>();

        services.AddSingleton<DiscordBot>();
        services.AddHostedService(provider => (DiscordBot)provider.GetService(typeof(DiscordBot))!);

        //services.AddHostedService<ThreadReviver>();

        services.AddHostedService<SaveStaticFile>();
        services.AddHostedService<Send>();
        services.AddHostedService<Publish>();
        services.AddHostedService<RewriteElectionsChannel>();
        services.AddHostedService<UpdateUser>();
    }
}