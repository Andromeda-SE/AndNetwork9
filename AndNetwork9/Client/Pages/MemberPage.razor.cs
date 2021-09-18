using System;
using System.Net.Http.Json;
using AndNetwork9.Client.Shared;
using AndNetwork9.Client.Utility;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Extensions;

namespace AndNetwork9.Client.Pages
{
    public partial class MemberPage : ElementPageBase<Member>
    {
        public override ColumnDefinition[] ColumnDefinitions { get; } =
        {
            new("Ранг",
                x => x.Rank,
                x => ClanRulesExtensions.GetRankIcon(x.Rank),
                _ => false,
                (SortType.Default, true)),
            new("Никнейм",
                x => x.Nickname,
                x => x.Nickname,
                _ => true,
                (SortType.Alphabet, false)),
            new("Имя",
                x => x.RealName,
                x => x.RealName,
                _ => false,
                (SortType.Alphabet, false)),
            new("Направление",
                x => x.Direction,
                x => ClanRulesExtensions.GetName(x.Direction),
                _ => false,
                (SortType.Default, true)),
            new("Отряд",
                x => x.SquadNumber,
                x => x.SquadNumber is not null ? RomanExtensions.ToRoman(x.SquadNumber) : "—",
                x => x.SquadNumber is not null && x.IsSquadCommander,
                (SortType.Numeric, false)),
        };

        protected override async void SetNewEntity(int? value)
        {
            Console.WriteLine(value?.ToString() ?? "null");
            SelectedEntity = value is null ? null : await Client.GetFromJsonAsync<Member>($"api/Member/{value}");
            if (SelectedEntity is not null)
            {
                SelectedEntity.Awards = (await Client.GetFromJsonAsync<Award[]>($"api/Award/{value}"))!;
                SelectedEntity.Squad = SelectedEntity.SquadNumber is null
                    ? null
                    : await Client.GetFromJsonAsync<Squad>($"api/squad/{SelectedEntity.SquadNumber}");
            }

            StateHasChanged();
        }

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            Entities = await Client.GetFromJsonAsync<Member[]>("api/member/all");
        }

        private void SelectionChanged(int? value)
        {
            NavigationManager.NavigateTo(value is null ? "member" : $"member/{value:D}");
        }
    }
}