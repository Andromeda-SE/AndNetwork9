using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using AndNetwork9.Client.Services;
using AndNetwork9.Client.Shared;
using AndNetwork9.Client.Utility;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Client.Pages.Squads;

public partial class MySquad
{
    private bool _isInitialized;
    private static Member[] _allMembers;
    public Squad Squad { get; set; }

    public short SelectedPartNumber
    {
        get => SelectedPart.Part;
        set
        {
            SelectedPart = Squad.SquadParts.Single(x => x.Part == value);
            Entities = SelectedPart.Members.ToArray();
            SetNewEntity(null);
        }
    }

    public SquadPart SelectedPart { get; set; }
    public override ColumnDefinition[] ColumnDefinitions { get; } =
    {
        new ("Игрок", x => x, x => x.ToString(), _ => true, (SortType.Default, true))
    };

    protected override void SetNewEntity(int? value)
    {
        SelectedEntity = value is null ? null : Entities.Single(x => x.Id == value);
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        if (AuthStateProvider.CurrentMember.SquadNumber is null) throw new Exception();
        _allMembers = await Client.GetFromJsonAsync<Member[]>($"api/member/all");
        Squad = await Client.GetFromJsonAsync<Squad>($"api/squad/{AuthStateProvider.CurrentMember.SquadNumber}");
        Squad.SquadParts = await Client.GetFromJsonAsync<SquadPart[]>($"api/squad/{AuthStateProvider.CurrentMember.SquadNumber}/parts");
        foreach ((SquadPart squadPart, IEnumerable<Member> members) in Squad.SquadParts.GroupJoin(
                     _allMembers.Where(x => x.SquadNumber == AuthStateProvider.CurrentMember.SquadNumber),
                     x => x.Number,
                     x => x.SquadPartNumber,
                     (part, members) => (part, members)))
        {
            foreach (var member in members)
            {
                member.SquadPart = squadPart;
            }
        }
        foreach (var squadPart in Squad.SquadParts)
        {
            squadPart.Members = _allMembers
                .Where(x => x.SquadNumber == Squad.Number && x.SquadPartNumber == squadPart.Part).ToArray();
        }


        SelectedPartNumber = 0;
        _isInitialized = true;
    }

    private void SelectionChanged(int? value)
    {
        SetNewEntity(value);
    }
}