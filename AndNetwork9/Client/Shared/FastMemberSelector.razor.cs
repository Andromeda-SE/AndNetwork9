using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Interfaces;
using Microsoft.AspNetCore.Components;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Client.Shared;

public partial class FastMemberSelector
{
    public enum FastEditType
    {
        One,
        Squad,
        SquadPart,
        Direction,
        VoteMembers,
        VoteVoted,
        SquadCommanders,
        Advisors,
        All,
    }

    [Inject]
    public HttpClient Client { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public string Id { get; set; }
    [Parameter]
    public IReadOnlyCollection<Member> InitialMembers { get; set; }
    public IList<Member> Members { get; set; }
    [Parameter]
    public IReadOnlyCollection<Squad> AllSquads { get; set; }
    public Squad SelectedSquad { get; set; }

    public int SelectedSquadId
    {
        get => SelectedSquad?.Number ?? 0;
        set { SelectedSquad = AllSquads.FirstOrDefault(x => x.Number == value); }
    }

    [Parameter]
    public IReadOnlyCollection<Member> AllMembers { get; set; }
    public Member SelectedMember { get; set; }

    public int SelectedMemberId
    {
        get => SelectedMember?.Id ?? 0;
        set { SelectedMember = AllMembers.FirstOrDefault(x => x.Id == value); }
    }

    public FastEditType SelectedEditType { get; set; }
    public Direction SelectedDirection { get; set; }
    [Parameter]
    public Action<IReadOnlyCollection<Member>> MembersUpdated { get; set; }

    private void Update()
    {
        MembersUpdated((IReadOnlyCollection<Member>)Members);
    }

    protected override async Task OnInitializedAsync()
    {
        Members = InitialMembers.ToList();
        AllSquads ??= await Client.GetFromJsonAsync<Squad[]>("api/squad/all");
        AllMembers ??= await Client.GetFromJsonAsync<Member[]>("api/member/all");

        foreach (Squad squad in AllSquads)
        {
            squad.SquadParts = await Client.GetFromJsonAsync<List<SquadPart>>($"api/squad/{squad.Number:D}/parts");
            foreach (SquadPart squadPart in squad.SquadParts)
                squadPart.Members = AllMembers
                    .Where(x => x.SquadNumber == squadPart.Number && x.SquadPartNumber == squadPart.Part).ToList();
        }
    }

    private void AddMembers(IEnumerable<Member> members)
    {
        foreach (Member member in members) Members.Add(member);
        StateHasChanged();
    }

    private void RemoveMembers(IEnumerable<Member> members)
    {
        foreach (Member member in members) Members.Remove(member);
        StateHasChanged();
    }
}

public interface IFastMemberSelectorEditType
{
    public IId SelectedEntity { get; }
    public IReadOnlyCollection<IId> Entities { get; }
    public IEnumerable<Member> Members();
}