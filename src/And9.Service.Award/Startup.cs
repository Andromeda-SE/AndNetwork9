using And9.Gateway.Clan.Senders;
using And9.Integration.Steam.Senders;
using And9.Lib.Broker;
using And9.Service.Award.Database;
using And9.Service.Award.Listeners;
using And9.Service.Award.Senders;
using And9.Service.Award.Services;
using And9.Service.Award.Services.AwardDispenser;
using And9.Service.Award.Services.AwardDispenser.Strategy;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Award;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        RabbitConnectionPool.SetConfiguration(configuration);
        services.AddSingleton(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddDbContext<AwardDataContext>(x => x.UseNpgsql(configuration["Postgres:ConnectionString"]));

        services.AddScoped<MemberCrudSender>();
        services.AddScoped<AwardCrudSender>();
        services.AddScoped<RaiseMemberUpdateSender>();
        services.AddScoped<PlayerActivitySender>();

        services.AddSingleton<InGameAwardDispenserStrategy>();
        services.AddSingleton<TogetherInGameAwardDispenserStrategy>();

        services.AddHostedService<RankUpdaterService>();
        services.AddHostedService<AwardDispenserService<InGameAwardDispenserStrategy>>();
        services.AddHostedService<AwardDispenserService<TogetherInGameAwardDispenserStrategy>>();

        services.AddHostedService<AwardModelCrudListener>();
    }
}