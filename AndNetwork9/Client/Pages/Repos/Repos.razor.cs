using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Client.Services;
using AndNetwork9.Client.Shared;
using AndNetwork9.Client.Utility;
using AndNetwork9.Shared.Extensions;
using AndNetwork9.Shared.Storage;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Repos
{
    public partial class Repos
    {
        public enum SelectedTab
        {
            All,
            Direction,
            Squad,
            Me,
        }

        [Inject]
        public HttpClient Client { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public SelectedTab Selected { get; set; }
        public AndNetwork9.Shared.Storage.Repo[] Entities { get; set; }
        public IEnumerable<AndNetwork9.Shared.Storage.Repo> ViewEntities => Selected switch
        {
            SelectedTab.All => Entities.OrderBy(x =>
                x.Nodes.Any() ? x.Nodes.Max(y => y.CreateTime) : DateTime.MinValue),
            SelectedTab.Direction => Entities
                .Where(x => x.Creator?.Direction == AuthStateProvider.CurrentMember.Direction).OrderBy(x =>
                    x.Nodes.Any() ? x.Nodes.Max(y => y.CreateTime) : DateTime.MinValue),
            SelectedTab.Squad => Entities
                .Where(x => x.Creator?.SquadNumber == AuthStateProvider.CurrentMember.SquadNumber).OrderBy(x =>
                    x.Nodes.Any() ? x.Nodes.Max(y => y.CreateTime) : DateTime.MinValue),
            SelectedTab.Me => Entities.Where(x => x.Creator?.Id == AuthStateProvider.CurrentMember.Id)
                .OrderBy(x => x.Nodes.Any() ? x.Nodes.Max(y => y.CreateTime) : DateTime.MinValue),
            _ => throw new ArgumentOutOfRangeException(),
        };
        public ColumnDefinition[] ColumnDefinitions { get; } =
        {
            new("Название",
                x => x.Name,
                x => x.Name,
                _ => false,
                (SortType.Alphabet, false)),
            new("Тип",
                x => x.Type,
                x => ClanRulesExtensions.GetString(x.Type),
                _ => false,
                (SortType.Alphabet, false)),
            new("Создатель",
                x => x.Creator,
                x => x.Creator?.ToString() ?? "Н/Д",
                _ => false,
                (SortType.Alphabet, false)),
            new("Последние изменение",
                x => ((IEnumerable<RepoNode>)x.Nodes).Any()
                    ? ((IEnumerable<RepoNode>)x.Nodes).Max(y => y.CreateTime)
                    : DateTime.MinValue,
                x => ((IEnumerable<RepoNode>)x.Nodes).Any()
                    ? ((IEnumerable<RepoNode>)x.Nodes).Max(y => y.CreateTime).ToString("G")
                    : string.Empty,
                _ => false,
                (SortType.Numeric, false)),
        };

        private void SelectionChanged(int? value)
        {
            NavigationManager.NavigateTo(value is null ? "repo" : $"repo/{value:D}");
        }

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            Entities = await Client.GetFromJsonAsync<AndNetwork9.Shared.Storage.Repo[]>("api/repo/");
        }
    }
}