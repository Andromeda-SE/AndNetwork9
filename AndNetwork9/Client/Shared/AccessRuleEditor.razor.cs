using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Components;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Client.Shared
{
    public partial class AccessRuleEditor
    {
        private bool _initialized = false;
        [Parameter]
        public string Id { get; set; }
        public Member[] Members { get; private set; }
        public List<Member> SelectedMembers { get; private set; }
        public Squad[] Squads { get; private set; }
        public int? SelectedSquadId { get; private set; }
        [Inject]
        public HttpClient Client { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public AccessRule Model { get; set; }
        [Parameter]
        public Action<AccessRule> RuleUpdated { get; set; }

        public Dictionary<Direction, bool> DirectionAccess { get; } =
            Enum.GetValues<Direction>().ToDictionary(x => x, _ => false);

        public Rank MinRank { get; set; } = Rank.Neophyte;
        public int? SelectedMemberId { get; set; }

        public Member SelectedMember
        {
            get => SelectedMemberId is null ? null : Members.First(x => x.Id == SelectedMemberId);
            set => SelectedMemberId = value?.Id;
        }

        public Squad SelectedSquad
        {
            get => SelectedSquadId is null ? null : Squads.First(x => x.Number == SelectedSquadId);
            set => SelectedSquadId = value?.Number;
        }

        protected override async Task OnInitializedAsync()
        {
            AccessRule rule = Model
                              ?? new()
                              {
                                  Name = null,
                                  AllowedMembers = new List<Member>(),
                                  Directions = Enum.GetValues<Direction>(),
                                  Id = 0,
                                  MinRank = Rank.Neophyte,
                                  Squad = null,
                                  SquadId = null,
                              };
            Squads = await Client.GetFromJsonAsync<Squad[]>("api/squad/all");
            Members = await Client.GetFromJsonAsync<Member[]>("api/member/all");

            MinRank = rule.MinRank;
            SelectedMembers = Model.Id > 0
                ? new(await Client.GetFromJsonAsync<List<Member>>($"api/AccessRule/{Model.Id}/overrides"))
                : new List<Member>();
            SelectedSquadId = Model.SquadId;
            foreach (Direction direction in rule.Directions) DirectionAccess[direction] = true;
            _initialized = true;
        }

        private async Task Save()
        {
            AccessRule newRule = new()
            {
                Name = null,
                MinRank = MinRank,
                AllowedMembers = SelectedMembers,
                Directions = DirectionAccess.Where(x => x.Value).Select(x => x.Key).ToArray(),
                Squad = SelectedSquad,
                SquadId = SelectedSquadId,
                AllowedMembersIds = SelectedMembers.Select(x => x.Id).ToArray(),
                Id = 0,
            };
            HttpResponseMessage response = await Client.PostAsJsonAsync("api/AccessRule", newRule);
            RuleUpdated(await response.Content.ReadFromJsonAsync<AccessRule>());
        }

        private async void Cancel()
        {
            await OnInitializedAsync();
            RuleUpdated(Model);
        }

        private void AddMember()
        {
            SelectedMembers.Add(SelectedMember);
            SelectedMemberId = null;
        }
    }
}