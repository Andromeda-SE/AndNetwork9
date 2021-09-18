using System;
using System.Linq;
using System.Net.Http.Json;
using AndNetwork9.Client.Shared;
using AndNetwork9.Client.Utility;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Extensions;

namespace AndNetwork9.Client.Pages
{
    public partial class SquadPage : ElementPageBase<Squad>
    {
        public override ColumnDefinition[] ColumnDefinitions { get; } =
        {
            new("Номер",
                x => -x.Number,
                x => RomanExtensions.ToRoman(x.Number),
                x => x.DisbandDate is null || x.DisbandDate > DateOnly.FromDateTime(DateTime.Today),
                (SortType.Numeric, true)),
            new("Название",
                x => x.Name,
                x => string.IsNullOrWhiteSpace(x.Name) ? "—" : x.Name,
                _ => false,
                (SortType.Alphabet, false)),
        };

        protected override async void SetNewEntity(int? value)
        {
            SelectedEntity = value is null ? null : await Client.GetFromJsonAsync<Squad>($"api/squad/{value}");
            if (SelectedEntity is not null && !SelectedEntity.Members.Any())
            {
                SelectedEntity.Members = (await Client.GetFromJsonAsync<Member[]>("api/member/all"))
                    ?.Where(x => x.SquadNumber == value).ToArray()!;
                StateHasChanged();
            }
        }

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            Entities = await Client.GetFromJsonAsync<Squad[]>("api/squad/all");
        }

        private void SelectionChanged(int? value)
        {
            NavigationManager.NavigateTo(value is null ? "squad" : $"squad/{value:D}");
        }
    }
}