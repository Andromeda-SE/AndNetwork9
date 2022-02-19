using And9.Lib.Broker;
using And9.Service.Election.Database;
using Microsoft.EntityFrameworkCore;

namespace And9.Service.Election;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        RabbitConnectionPool.SetConfiguration(configuration);
        services.AddSingleton(_ => RabbitConnectionPool.Factory.CreateConnection());

        services.AddDbContext<ElectionDataContext>(x => x.UseNpgsql(configuration["Postgres:ConnectionString"]));
    }
}