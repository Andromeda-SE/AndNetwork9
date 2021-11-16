using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Shared;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Shared;

public partial class TaskShortCard
{
    [Inject]
    public HttpClient Client { get; set; }

    [Parameter]
    public Task Task { get; set; }

    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        Task.Assignee ??= Task.AssigneeId is not null
                ? await Client.GetFromJsonAsync<Member>($"api/member/{Task.AssigneeId}")
                : null;
    }
}