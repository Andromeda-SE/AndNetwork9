using And9.Lib.Broker;
using And9.Service.Core.Database;
using And9.Service.Core.Listeners;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Core;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        RabbitConnectionPool.SetConfiguration(configuration);
        services.AddSingleton(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddDbContext<CoreDataContext>(x => x.UseNpgsql(configuration["Postgres:ConnectionString"]));

        services.AddSingleton<MemberCrudSender>();

        services.AddHostedService<AcceptCandidateListener>();
        services.AddHostedService<DeclineCandidateListener>();
        services.AddHostedService<RegisterCandidateRequestListener>();
        services.AddHostedService<MemberCrudListener>();
        services.AddHostedService<CandidateRequestListener>();
        services.AddHostedService<ReadMemberBySteamIdListener>();
        services.AddHostedService<ReadMemberByDiscordIdListener>();
        services.AddHostedService<ReadMemberByNicknameListener>();
    }
}