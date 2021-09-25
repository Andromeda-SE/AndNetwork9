using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Elections;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Elections
{
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
            StateHasChanged();
        }

        private async System.Threading.Tasks.Task Register()
        {
            HttpResponseMessage response = await Client.GetAsync("api/Election/reg");
            if (response.IsSuccessStatusCode) NavigationManager.NavigateTo("/election", true);
        }
    }
}