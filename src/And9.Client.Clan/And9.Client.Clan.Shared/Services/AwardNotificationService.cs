#if WINDOWS
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using And9.Gateway.Clan.Client.Connections;
using And9.Service.Award.Abstractions;
using And9.Service.Award.Abstractions.Models;
using And9.Service.Core.Abstractions.Models;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using And9.Client.Clan.Config.Services;
using Microsoft.Extensions.DependencyInjection;

namespace And9.Client.Clan.Services;

public class AwardNotificationService : IHostedService
{
    private readonly RepositoryConnection<Award> _awards;
    private readonly CoreConnection _coreConnection;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AwardNotificationService(RepositoryConnection<Award> awards, CoreConnection coreConnection, IServiceScopeFactory serviceScopeFactory)
    {
        _awards = awards;
        _coreConnection = coreConnection;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _awards.CollectionChanged += AwardsOnCollectionChanged;
        if (_coreConnection.Connection.State == HubConnectionState.Disconnected) await _coreConnection.StartAsync(CancellationToken.None);
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _awards.CollectionChanged -= AwardsOnCollectionChanged;
        return Task.CompletedTask;
    }

    private async void AwardsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        ApplicationDataService applicationDataService = scope.ServiceProvider.GetRequiredService<ApplicationDataService>();

        Member? me = await _coreConnection.ReadMe();
        if (me is null) return;
        IEnumerable<Award>? awards;
        if (e.Action == NotifyCollectionChangedAction.Reset)
            awards = _awards.Where(x => x.MemberId == me.Id);
        else
            awards = e.NewItems?.OfType<Award>().Where(x => x.MemberId == me.Id);
        if (awards is null) return;
        foreach (Award award in awards.ExceptBy(applicationDataService.GaveAwards.Select(x => x.Item1), award => award.Id))
        {
            ToastContent toastContent = new()
            {
                Visual = new()
                {
                    BindingGeneric = new()
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = "Вы получили новую награду",
                            },
                            new AdaptiveText
                            {
                                Text = award.Type.GetDisplayString(),
                            },
                            new AdaptiveText
                            {
                                Text = award.Description,
                            },
                        },
                    },
                },
                Actions = new ToastActionsCustom
                {
                    Buttons =
                    {
                        new ToastButtonDismiss(),
                    },
                },
                Launch = $"action={nameof(AwardNotificationService)}",
                ActivationType = ToastActivationType.Background,
            };
            ToastNotification notification = new(toastContent.GetXml());
            notification.Activated += NotificationOnActivated;
            ToastNotificationManager.CreateToastNotifier().Show(notification);
            await applicationDataService.GaveAwards.AddAsync(new(award.Id));
        }

        await applicationDataService.SaveChangesAsync();
    }

    private void NotificationOnActivated(ToastNotification sender, object args) { }
}
#endif