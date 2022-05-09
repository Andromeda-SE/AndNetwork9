using System.Reflection;
using And9.Gateway.Clan.Senders;
using And9.Integration.Steam.Senders;
using And9.Lib.Broker;
using And9.Service.Award.ConsumersStrategies;
using And9.Service.Award.Database;
using And9.Service.Award.Jobs;
using And9.Service.Award.Jobs.AwardDispense;
using And9.Service.Award.Jobs.AwardDispense.Strategy;
using And9.Service.Award.Senders;
using And9.Service.Core.Senders;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Quartz;

namespace And9.Service.Award;

public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AwardDataContext>(x => x.UseNpgsql(Configuration["Postgres:ConnectionString"]));

        services.WithBroker(Configuration)
            .AppendConsumerWithResponse<CreateAwardConsumerStrategy, Abstractions.Models.Award, int>()
            .AppendConsumerWithResponse<ReadAwardConsumerStrategy, int, Abstractions.Models.Award?>()
            .AppendConsumerWithCollectionResponse<ReadAllAwardConsumerStrategy, int, Abstractions.Models.Award>()
            .AppendConsumerWithCollectionResponse<ReadByMemberIdAwardConsumerStrategy, int, Abstractions.Models.Award>()
            .AddGatewaySenders()
            .AddAwardSenders()
            .AddSteamSenders()
            .AddCoreSenders()
            .Build();

        services.AddScoped<InGameAwardDispenserStrategy>();
        services.AddScoped<TogetherInGameAwardDispenserStrategy>();

        services.AddScoped<AwardDispenseJob<InGameAwardDispenserStrategy>>();
        services.AddScoped<AwardDispenseJob<TogetherInGameAwardDispenserStrategy>>();
        services.AddScoped<RankUpdateJob>();

        services.AddQuartz(configurator =>
        {
            configurator.SchedulerName = configurator.SchedulerId = Assembly.GetEntryAssembly()?.FullName ?? throw new InvalidOperationException();
            configurator.UseMicrosoftDependencyInjectionJobFactory();
            configurator.UseDedicatedThreadPool(1);
            //configurator.UseSimpleTypeLoader();
            configurator.UsePersistentStore(options =>
            {
                options.UseClustering();
                options.UseSerializer<JsonQuartzSerializer>();
                options.Properties["quartz.jobStore.tablePrefix"] = "\"Award\".qrtz_";
                options.UsePostgres(Configuration["Postgres:ConnectionString"] + "Search Path=Award;");
            });

            DateTimeOffset now = DateTimeOffset.UtcNow;
            configurator.ScheduleJob<RankUpdateJob>(triggerConfigurator =>
            {
                triggerConfigurator.WithIdentity(nameof(RankUpdateJob) + "_Trigger");
                triggerConfigurator.StartNow();
                triggerConfigurator.WithSimpleSchedule(builder =>
                {
                    builder.WithIntervalInSeconds(10);
                    builder.RepeatForever();
                });
            }, jobConfigurator => jobConfigurator.WithIdentity(nameof(RankUpdateJob) + "_Job"));
            configurator.ScheduleJob<AwardDispenseJob<InGameAwardDispenserStrategy>>(triggerConfigurator =>
            {
                triggerConfigurator.WithIdentity(nameof(InGameAwardDispenserStrategy) + "_Trigger");
                triggerConfigurator.StartAt(now.AddSeconds(60 + 5));
                triggerConfigurator.WithSimpleSchedule(builder =>
                {
                    builder.WithIntervalInMinutes(3);
                    builder.RepeatForever();
                });
            }, jobConfigurator => jobConfigurator.WithIdentity(nameof(InGameAwardDispenserStrategy) + "_Job"));
            configurator.ScheduleJob<AwardDispenseJob<TogetherInGameAwardDispenserStrategy>>(triggerConfigurator =>
            {
                triggerConfigurator.WithIdentity(nameof(TogetherInGameAwardDispenserStrategy) + "_Trigger");
                triggerConfigurator.StartAt(now.AddSeconds(60 + 10));
                triggerConfigurator.WithSimpleSchedule(builder =>
                {
                    builder.WithIntervalInMinutes(3);
                    builder.RepeatForever();
                });
            }, jobConfigurator => jobConfigurator.WithIdentity(nameof(TogetherInGameAwardDispenserStrategy) + "_Job"));
        });
        services.AddQuartzHostedService(options =>
        {
            options.StartDelay = null;
            options.AwaitApplicationStarted = true;
            options.WaitForJobsToComplete = true;
        });

        services.AddHealthChecks()
            .AddDbContextCheck<AwardDataContext>()
            .AddRabbitMQ()
            .ForwardToPrometheus();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseRouting();
        app.UseHttpMetrics();
        app.UseMetricServer();
        app.UseHealthChecks("/health");
    }
}