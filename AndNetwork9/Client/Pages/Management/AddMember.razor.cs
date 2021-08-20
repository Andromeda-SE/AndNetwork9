using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Management
{
    public partial class AddMember
    {
        [Inject]
        public HttpClient Client { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public Member Model { get; set; } = new()
        {
            Rank = Rank.Guest,
        };

        private bool Validate() => !string.IsNullOrWhiteSpace(Model.Nickname)
                                   && Model.SteamId != 0
                                   && Model.DiscordId != 0;

        private async System.Threading.Tasks.Task Create()
        {
            HttpResponseMessage result = await Client.PostAsJsonAsync("api/member", Model);
            if (result.IsSuccessStatusCode)
            {
                Member member = await result.Content.ReadFromJsonAsync<Member>();
                NavigationManager.NavigateTo($"member/{member.Id}");
            }
        }
    }
}