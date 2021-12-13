using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Client.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Tasks;

[Authorize]
public partial class Task
{
    [Inject]
    public HttpClient Client { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    [Parameter]
    public int TaskId { get; set; }
    public AndNetwork9.Shared.Task Model { get; set; }
    public string EditedText { get; set; } = string.Empty;
    protected MarkupString Description => Model.Description.Text.FromMarkdown();
    public Member[] AllMembers { get; set; }
    public Squad[] AllSquads { get; set; }

    protected bool SendEnabled => !string.IsNullOrWhiteSpace(Model.Title);

    protected virtual async System.Threading.Tasks.Task UpdateDescription()
    {
        Model.Description = await (await Client.PutAsJsonAsync($"api/comment/{Model.DescriptionId}",
            Model.Description with
            {
                Text = EditedText,
            })).Content.ReadFromJsonAsync<Comment>();
    }

    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        Model = await Client.GetFromJsonAsync<AndNetwork9.Shared.Task>($"api/task/{TaskId}");
        Model.ReadRule = await Client.GetFromJsonAsync<AccessRule>($"api/AccessRule/{Model.ReadRuleId}");
        Model.WriteRule = await Client.GetFromJsonAsync<AccessRule>($"api/AccessRule/{Model.WriteRuleId}");
        EditedText = Model.Description.Text;
        if (Model.ParentId is not null)
            Model.Parent = await Client.GetFromJsonAsync<AndNetwork9.Shared.Task>($"api/task/{Model.ParentId}");
        Model.Children = await Client.GetFromJsonAsync<AndNetwork9.Shared.Task[]>($"api/task/{TaskId}/children");

        AllMembers = await Client.GetFromJsonAsync<Member[]>("api/member/all");
        AllSquads = await Client.GetFromJsonAsync<Squad[]>("api/squad/all");

        foreach (Squad squad in AllSquads)
            squad.SquadParts = await Client.GetFromJsonAsync<SquadPart[]>($"api/squad/{squad.Number}/parts");

        foreach (Member member in AllMembers)
        {
            if (member.SquadNumber is null) continue;
            member.SquadPart = AllSquads.First(x => x.Number == member.SquadNumber).SquadParts
                .First(x => x.Part == member.SquadPartNumber);
        }

        await base.OnInitializedAsync();
    }

    protected async void UpdateReadRule(AccessRule rule)
    {
        if (Model.ReadRuleId == rule.Id) return;
        Model.ReadRuleId = rule.Id;
        Model.ReadRule = await Client.GetFromJsonAsync<AccessRule>($"api/AccessRule/{Model.ReadRuleId}");
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    protected async void UpdateWriteRule(AccessRule rule)
    {
        if (Model.WriteRuleId == rule.Id) return;
        Model.WriteRuleId = rule.Id;
        Model.WriteRule = await Client.GetFromJsonAsync<AccessRule>($"api/AccessRule/{Model.WriteRuleId}");
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    protected virtual async System.Threading.Tasks.Task Send()
    {
        await Client.PutAsJsonAsync($"api/task/{TaskId}", Model);
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }
}