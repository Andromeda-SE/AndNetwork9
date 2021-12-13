using System.Linq;
using System.Net.Http.Json;
using AndNetwork9.Client.Shared;
using AndNetwork9.Client.Utility;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Client.Pages.Squads;

public partial class Squads
{
    private static Member[] _members;
    public override ColumnDefinition[] ColumnDefinitions { get; } =
    {
        new("#",
            x => x.Number,
            x => RomanExtensions.ToRoman(x.Number),
            _ => true,
            (SortType.Numeric, true)),
        new("Эпитет",
            x => x.Name,
            x => x.Name,
            _ => true,
            (SortType.Alphabet, false)),
        new("Капитан",
            x => x.Captain,
            x => x.Captain.ToString(),
            _ => false,
            (SortType.Default, false)),
        new("Личный состав",
            x => _members.Count(y => y.SquadNumber == x.Number),
            x =>
            {
                int auxiliaryCount = _members.Count(y => y.Rank <= Rank.None && y.SquadNumber == x.Number);
                return auxiliaryCount == 0
                    ? $"{_members.Count(y => y.Rank > Rank.None && y.SquadNumber == x.Number):D}"
                    : $"{_members.Count(y => y.Rank > Rank.None && y.SquadNumber == x.Number):D} + {auxiliaryCount:D}";
            },
            _ => false,
            (SortType.Numeric, false)),
    };

    protected override async void SetNewEntity(int? value)
    {
        SelectedEntity = value is null ? null : Entities.Single(x => x.Number == value);
        if (SelectedEntity is not null) { }

        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        _members = await Client.GetFromJsonAsync<Member[]>("api/member/all");

        Squad[] entities = await Client.GetFromJsonAsync<Squad[]>("api/squad/all");

        foreach (Squad entity in entities)
        {
            entity.SquadParts = await Client.GetFromJsonAsync<SquadPart[]>($"api/squad/{entity.Number}/parts");
            foreach (SquadPart part in entity.SquadParts)
            {
                part.Members = _members.Where(x => x.SquadNumber == entity.Number && x.SquadPartNumber == part.Part)
                    .ToArray();
                part.Commander = _members.First(x => x.Id == part.CommanderId);
                part.Commander.SquadPart = part;
            }
        }

        foreach (Member member in _members.Where(x => x.SquadNumber is not null))
            member.SquadPart = entities.First(x => x.Number == member.SquadNumber).SquadParts
                .First(x => x.Part == member.SquadPartNumber);

        Entities = entities;
    }

    private void SelectionChanged(int? value)
    {
        NavigationManager.NavigateTo(value is null ? "squad" : $"squad/{value:D}");
    }
}