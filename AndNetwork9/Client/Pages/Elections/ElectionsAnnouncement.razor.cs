using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Client.Services;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Elections;
using AndNetwork9.Shared.Interfaces;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Elections;

public partial class ElectionsAnnouncement : IDisposable
{
    [Inject]
    public HttpClient Client { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    [Inject]
    public ModelService ModelService { get; set; }
    [Parameter]
    public CouncilElection CurrentElections { get; set; }
    [Parameter]
    public IReadOnlyCollection<Member> AllMembers { get; set; }
    public bool Initialized { get; set; } = false;

    public void Dispose()
    {
        ModelService.Received -= OnModelServiceOnReceived;
    }

    protected override async void OnInitialized()
    {
        CurrentElections ??= await Client.GetFromJsonAsync<CouncilElection>("api/Election/current");
        AllMembers ??= await Client.GetFromJsonAsync<Member[]>("api/member/all");
        foreach (Member entity in AllMembers)
            entity.SquadPart = entity.SquadNumber is null
                ? null
                : await Client.GetFromJsonAsync<SquadPart>(
                    $"api/squad/{entity.SquadNumber}/part/{entity.SquadPartNumber}");
        Initialized = true;

        await ModelService.StartAsync();
        ModelService.Received += OnModelServiceOnReceived;

        StateHasChanged();
    }

    private void OnModelServiceOnReceived(string type, IId id)
    {
        if (type != typeof(CouncilElection).FullName || id.Id != CurrentElections.Id) return;
        CurrentElections = (CouncilElection)id;
        StateHasChanged();
    }
}