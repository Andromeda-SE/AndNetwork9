using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Elections;
using Microsoft.AspNetCore.Components;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Client.Pages.Elections;

public partial class ElectionsRegistration
{
    [Inject]
    public HttpClient Client { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    [Parameter]
    public CouncilElection CurrentElections { get; set; }
    [Parameter]
    public IReadOnlyCollection<Member> AllMembers { get; set; }

    protected override async void OnInitialized()
    {
        CurrentElections ??= await Client.GetFromJsonAsync<CouncilElection>("api/Election/current");
        AllMembers ??= await Client.GetFromJsonAsync<Member[]>("api/member/all");
        foreach (Member entity in AllMembers)
            entity.SquadPart = entity.SquadNumber is null
                ? null
                : await Client.GetFromJsonAsync<SquadPart>(
                    $"api/squad/{entity.SquadNumber}/part/{entity.SquadPartNumber}");
        StateHasChanged();
    }

    private async Task Register()
    {
        HttpResponseMessage response = await Client.GetAsync("api/Election/reg");
        if (response.IsSuccessStatusCode) NavigationManager.NavigateTo("/election", true);
    }
}