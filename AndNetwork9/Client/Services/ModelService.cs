using System;
using System.Threading.Tasks;
using AndNetwork9.Client.Utility;
using AndNetwork9.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace AndNetwork9.Client.Services;

public class ModelService
{
    private HubConnection _hubConnection;

    public ModelService(NavigationManager navigationManager) => NavigationManager = navigationManager;
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public event Action<string, IId> Received;

    internal async Task StartAsync()
    {
        if (_hubConnection is not null) return;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("hub/model"),
                options =>
                {
                    options.HttpMessageHandlerFactory = handler => new IncludeRequestCredentialsMessageHandler
                    {
                        InnerHandler = handler,
                    };
                })
            .WithAutomaticReconnect()
            .Build();
        _hubConnection.On<string, IId>("ReceiveModelUpdate", (type, model) => Received?.Invoke(type, model));

        await _hubConnection.StartAsync();
    }
}