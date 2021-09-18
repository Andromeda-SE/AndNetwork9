using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Client.Services;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Management
{
    public partial class AddAward
    {
        [Inject]
        public HttpClient Client { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Parameter]
        public AwardType Type { get; set; } = AwardType.Bronze;
        [Parameter]
        public string Description { get; set; } = string.Empty;
        [Parameter]
        public List<Member> SelectedMembers { get; set; } = new();

        private void UpdateMembersList(IReadOnlyCollection<Member> members)
        {
            SelectedMembers = members.ToList();
            StateHasChanged();
        }

        private bool Validate() => SelectedMembers.Any() && !string.IsNullOrWhiteSpace(Description);

        private async System.Threading.Tasks.Task Create()
        {
            Award[] awards = SelectedMembers.Select(x => new Award
            {
                Date = DateOnly.FromDateTime(DateTime.Today),
                Description = Description,
                GaveById = AuthStateProvider.CurrentMember.Id,
                Type = Type,
                MemberId = x.Id,
            }).ToArray();
            HttpResponseMessage result = await Client.PostAsJsonAsync("api/award", awards);
            if (result.IsSuccessStatusCode) NavigationManager.NavigateTo($"member/{awards.First().Id}");
        }
    }
}