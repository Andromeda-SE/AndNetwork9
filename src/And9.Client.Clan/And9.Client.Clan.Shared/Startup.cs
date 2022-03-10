using And9.Client.Clan.Services;
using And9.Client.Clan.ViewModels;
using And9.Client.Clan.Views;
using And9.Client.Clan.Views.Dialogs;
using And9.Client.Clan.Views.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace And9.Client.Clan;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDataService>();
#if WINDOWS
        services.AddScoped<IConfiguration, ApplicationDataService>(provider => provider.GetRequiredService<ApplicationDataService>());

        services.AddHostedService<AwardNotificationService>();
        services.AddHostedService<RankNotificationService>();
#endif

        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<NavigationViewModel>();
        services.AddTransient<MembersViewModel>();
        services.AddTransient<GiveAwardViewModel>();

        services.AddSingleton<LoginPage>();
        services.AddSingleton<NavigationPage>();
        services.AddSingleton<MembersView>();
        services.AddTransient<GiveAwardContentDialog>();
    }
}