using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Shared.Elections;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Elections;

public partial class Elections
{
    [Inject]
    public HttpClient Client { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    [Parameter]
    public CouncilElection CurrentElections { get; set; }

    protected override async void OnInitialized()
    {
        CurrentElections ??= await Client.GetFromJsonAsync<CouncilElection>("api/Election/current");
        StateHasChanged();
    }
}