using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Service.Core.Senders;
using And9.Service.Election.Database;
using And9.Service.Election.Listeners;
using And9.Service.Election.Services.ElectionWatcher;
using And9.Service.Election.Services.ElectionWatcher.Strategies;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Election;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        RabbitConnectionPool.SetConfiguration(configuration);
        services.AddSingleton(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddDbContext<ElectionDataContext>(x => x.UseNpgsql(configuration["Postgres:ConnectionString"]));

        services.AddSingleton<MemberCrudSender>();
        services.AddSingleton<SendLogMessageSender>();

        services.AddTransient<NewElectionStrategy>();
        services.AddTransient<StartAnnouncementStrategy>();
        services.AddTransient<StartVoteStrategy>();

        services.AddHostedService<ElectionWatcher>();

        services.AddHostedService<CurrentElectionListener>();
        services.AddHostedService<RegisterListener>();
        services.AddHostedService<CancelRegisterListener>();
        services.AddHostedService<VoteListener>();
    }
}