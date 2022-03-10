#if WINDOWS
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using And9.Gateway.Clan.Client.Connections;
using And9.Service.Award.Abstractions.Models;
using And9.Service.Core.Abstractions;
using And9.Service.Core.Abstractions.Models;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.Extensions.Hosting;

namespace And9.Client.Clan.Services;

public class RankNotificationService : IHostedService
{
    private readonly CoreConnection _coreConnection;
    private readonly RepositoryConnection<Member> _members;

    public RankNotificationService(RepositoryConnection<Member> members, CoreConnection coreConnection)
    {
        _members = members;
        _coreConnection = coreConnection;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _members.CollectionChanged += AwardsOnCollectionChanged;
        return Task.CompletedTask;
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _members.CollectionChanged -= AwardsOnCollectionChanged;
        return Task.CompletedTask;
    }

    private async void AwardsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Member me = await _coreConnection.ReadMe();
        if (e.NewItems is null) return;
        if (e.NewItems.OfType<Award>().Any(x => x.MemberId == me.Id))
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
                                Text = "Вы получили новый ранг",
                            },
                            new AdaptiveText
                            {
                                Text = me.Rank.GetDisplayString(),
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
                Launch = $"action={nameof(RankNotificationService)}",
                ActivationType = ToastActivationType.Background,
            };
            ToastNotification notification = new(toastContent.GetXml());
            notification.Activated += NotificationOnActivated;
            ToastNotificationManager.CreateToastNotifier().Show(notification);
        }
    }

    private void NotificationOnActivated(ToastNotification sender, object args) { }
}
#endif