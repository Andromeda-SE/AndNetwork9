#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Tasks;

public partial class TaskList
{
    public enum ViewType
    {
        Me,
        SquadPart,
        Squad,
        Direction,
        Unassigned,
    }

    private bool _initiatorMode;

    [Inject]
    public HttpClient Client { get; set; }

    public bool InitiatorMode
    {
        get => _initiatorMode;
        set
        {
            _initiatorMode = value;
            LoadTasks();
        }
    }

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
        if (InitiatorMode)
        {
            AndNetwork9.Shared.Task[] tasks = await Client.GetFromJsonAsync<AndNetwork9.Shared.Task[]>(
                "api/task/me/reporter");
            AssigneeActive = tasks.Where(x =>
                    x.AssigneeId is null
                    || x.Status is TaskStatus.New
                        or TaskStatus.Analysis
                        or TaskStatus.Resolved)
                .ToArray();
            AssigneeOnInitiator = tasks.Where(x =>
                    x.AssigneeId is null
                    || x.Status is > TaskStatus.Analysis and < TaskStatus.Resolved)
                .ToArray();
            AssigneeEnd = tasks.Except(AssigneeActive).Except(AssigneeOnInitiator).ToArray();
        }
        else
        {
            switch (CurrentViewType)
            {
                case ViewType.Me:
                case ViewType.SquadPart:
                case ViewType.Squad:
                case ViewType.Direction:
                    AssigneeActive =
                        await Client.GetFromJsonAsync<List<AndNetwork9.Shared.Task>>(
                            $"api/task/{CurrentViewType:G}/assignee/active");
                    StateHasChanged();
                    AssigneeOnInitiator =
                        await Client.GetFromJsonAsync<List<AndNetwork9.Shared.Task>>(
                            $"api/task/{CurrentViewType:G}/assignee/analysis");
                    AssigneeEnd =
                        await Client.GetFromJsonAsync<List<AndNetwork9.Shared.Task>>(
                            $"api/task/{CurrentViewType:G}/assignee/closed");
                    break;
                case ViewType.Unassigned:
                    AssigneeActive =
                        await Client.GetFromJsonAsync<List<AndNetwork9.Shared.Task>>("api/task/unassigned");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        StateHasChanged();
    }
}