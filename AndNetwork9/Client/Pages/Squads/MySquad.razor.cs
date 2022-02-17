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
    public Squad Squad { get; set; }

    public short SelectedPartNumber
    {
        get => SelectedPart.Part;
        set
        {
            Console.WriteLine(1);
            if (!_isInitialized) return;
            SelectedPart = Squad.SquadParts.Single(x => x.Part == value);
            SetNewEntity(null);
        }
    }

    public SquadPart SelectedPart { get; set; }
    public override ColumnDefinition[] ColumnDefinitions { get; } =
    {
        new ("Должность", x => x.CommanderLevel, x => ClanRulesExtensions.GetSquadCommanderName(x.CommanderLevel), x => x.CommanderLevel != SquadCommander.None, (SortType.Default, false)),
        new ("Игрок", x => x, x => x.ToString(), x => x.CommanderLevel != SquadCommander.None, (SortType.Default, false))
    };

    protected override void SetNewEntity(int? value)
    {
        SelectedEntity = value is null ? null : SelectedPart.Members.Single(x => x.Id == value);
        if (_isInitialized) StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        if (AuthStateProvider.CurrentMember.SquadNumber is null) throw new Exception();
        Member[] allMembers = await Client.GetFromJsonAsync<Member[]>($"api/member/all");
        Squad = await Client.GetFromJsonAsync<Squad>($"api/squad/{AuthStateProvider.CurrentMember.SquadNumber}");
        Squad.SquadParts = await Client.GetFromJsonAsync<SquadPart[]>($"api/squad/{AuthStateProvider.CurrentMember.SquadNumber}/parts");
        foreach ((SquadPart squadPart, Member[] members) in Squad.SquadParts.GroupJoin(
                     allMembers.Where(x => x.SquadNumber == AuthStateProvider.CurrentMember.SquadNumber).ToArray(),
                     x => x.Part,
                     x => x.SquadPartNumber,
                     (part, members) => (part, members.ToArray())))
        {
            foreach (Member member in members)
            {
                member.SquadPart = squadPart;
            }
        }
        foreach (var squadPart in Squad.SquadParts)
        {
            squadPart.Members = allMembers
                .Where(x => x.SquadNumber == Squad.Number && x.SquadPartNumber == squadPart.Part).ToArray();
        }
        _isInitialized = true;
        if (SelectedPart is null) SelectedPartNumber = 0;
    }

    private void SelectionChanged(int? obj)
    {
        SetNewEntity(obj);
    }
}