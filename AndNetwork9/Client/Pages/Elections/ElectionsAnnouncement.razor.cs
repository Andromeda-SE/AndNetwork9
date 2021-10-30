using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Elections;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Elections;

public partial class ElectionsAnnouncement
{
    [Inject]
    public HttpClient Client { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    [Parameter]
    public CouncilElection CurrentElections { get; set; }
    [Parameter]
    public IReadOnlyCollection<Member> AllMembers { get; set; }
    public bool Initialized { get; set; } = false;

    protected override async void OnInitialized()
    {
        CurrentElections ??= await Client.GetFromJsonAsync<CouncilElection>("api/Election/current");
        AllMembers ??= await Client.GetFromJsonAsync<Member[]>("api/member/all");
        Initialized = true;
        StateHasChanged();
    }
}