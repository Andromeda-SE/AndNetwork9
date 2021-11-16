#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Tasks
{
    public partial class TaskList
    {
        [Inject]
        public HttpClient Client { get; set; }

        public bool InitiatorMode { get; set; } = false;

        public ViewType CurrentViewType { get; set; }
        public int CurrentViewTypeInt
        {
            get => (int)CurrentViewType;
            set
            {
                CurrentViewType = (ViewType)value;
                LoadTasks();
            }
        }

        public IList<AndNetwork9.Shared.Task>? AssigneeActive { get; set; }
        public IList<AndNetwork9.Shared.Task>? AssigneeOnInitiator { get; set; }
        public IList<AndNetwork9.Shared.Task>? AssigneeEnd { get; set; }

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            LoadTasks();
            await base.OnInitializedAsync();

        }

        private async void LoadTasks()
        {
            AssigneeActive = null;
            AssigneeOnInitiator = null;
            AssigneeEnd = null;
            switch (CurrentViewType)
            {
                case ViewType.Me:
                case ViewType.SquadPart:
                case ViewType.Squad:
                case ViewType.Direction:
                    AssigneeActive =
                        await Client.GetFromJsonAsync<List<AndNetwork9.Shared.Task>>($"api/task/{CurrentViewType:G}/assignee/active");
                    AssigneeOnInitiator =
                        await Client.GetFromJsonAsync<List<AndNetwork9.Shared.Task>>($"api/task/{CurrentViewType:G}/assignee/analysis");
                    AssigneeEnd =
                        await Client.GetFromJsonAsync<List<AndNetwork9.Shared.Task>>($"api/task/{CurrentViewType:G}/assignee/closed");
                    break;
                case ViewType.Unassigned:
                    AssigneeActive = 
                        await Client.GetFromJsonAsync<List<AndNetwork9.Shared.Task>>($"api/task/unassigned");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            StateHasChanged();
        }

        public enum ViewType
        {
            Me,
            SquadPart,
            Squad,
            Direction,
            Unassigned,
        }
    }
}
