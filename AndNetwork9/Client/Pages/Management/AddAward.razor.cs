using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Client.Services;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Client.Pages.Management;

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

    public AwardType[] AvailableTypes { get; set; }

    private void UpdateMembersList(IReadOnlyCollection<Member> members)
    {
        SelectedMembers = members.ToList();
        StateHasChanged();
    }

    protected override Task OnInitializedAsync()
    {
        switch (AuthStateProvider.CurrentMember.Rank)
        {
            case Rank.FirstAdvisor:
                AvailableTypes = Enum.GetValues<AwardType>().Where(x => x != AwardType.None).ToArray();
                return Task.CompletedTask;
            case Rank.Advisor:
                AvailableTypes = new[] {AwardType.Copper, AwardType.Bronze};
                return Task.CompletedTask;
            default:
            {
                if (AuthStateProvider.CurrentMember.CommanderLevel == SquadCommander.Captain)
                {
                    AvailableTypes = new[] {AwardType.Copper};
                    return Task.CompletedTask;
                }

                throw new();
            }
        }
    }

    private bool Validate() => SelectedMembers.Any() && !string.IsNullOrWhiteSpace(Description);

    private async Task Create()
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