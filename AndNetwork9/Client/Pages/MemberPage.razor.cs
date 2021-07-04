using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Client.Shared;
using AndNetwork9.Client.Utility;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Extensions;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages
{
    public partial class MemberPage
    {
        public Member[] Members { get; set; }

        [Parameter]
        public string SelectedKeyRaw
        {
            get => SelectedMember.Id.ToString("D");
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out int key))
                    SetNewMember(key);
                else
                    SetNewMember(null);
            }
        }

        [Inject]
        public HttpClient Client { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public Member SelectedMember { get; private set; }
        public Award[] SelectedMemberAwards { get; private set; }
        public ColumnDefinition[] ColumnDefinitions { get; } =
        {
            new("Ранг", x => x.Rank, x => ClanRulesExtensions.GetRankIcon(x.Rank), _ => false, (SortType.Default, true)),
            new("Никнейм", x => x.Nickname, x => x.Nickname, _ => true, (SortType.Alphabet, false)),
            new("Имя", x => x.RealName, x => x.RealName, _ => false, (SortType.Alphabet, false)),
            new("Направление", x => x.Direction, x => ClanRulesExtensions.GetName(x.Direction), _ => false, (SortType.Default, true)),
            new("Отряд", x => x.SquadNumber, x => x.SquadNumber is not null ? RomanExtensions.ToRoman(x.SquadNumber) : "—", _ => false, (SortType.Numeric, false)),
        };

        private async void SetNewMember(int? value)
        {
            SelectedMember = value is null ? null : await Client.GetFromJsonAsync<Member>($"api/Member/{value}");
            SelectedMemberAwards = value is null ? null : await Client.GetFromJsonAsync<Award[]>($"api/Award/{value}");
            StateHasChanged();
        }

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            Members = await Client.GetFromJsonAsync<Member[]>("api/member/all");
        }

        private void SelectionChanged(int? value)
        {
            NavigationManager.NavigateTo(value is null ? "member" : $"member/{value:D}");
        }
    }
}