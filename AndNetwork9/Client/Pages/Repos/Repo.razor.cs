using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AndNetwork9.Client.Extensions;
using AndNetwork9.Shared.Storage;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Repos;

public partial class Repo
{
    private bool _loaderOpened;
    [Inject]
    public HttpClient Client { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    [Parameter]
    public int EntityId { get; set; }
    [Parameter]
    public AndNetwork9.Shared.Storage.Repo Entity { get; set; }
    [Parameter]
    public int SelectedNodeIndex { get; set; }
    public string EditedText { get; set; }

    public RepoNode SelectedNode
    {
        get
        {
            try
            {
                return Entity.Nodes[SelectedNodeIndex];
            }
            catch
            {
                return null;
            }
        }
    }

    private MarkupString Description => Entity.Description.Text.FromMarkdown();

    [Parameter]
    public bool LoaderOpened
    {
        get => _loaderOpened;
        set
        {
            _loaderOpened = value;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        AndNetwork9.Shared.Storage.Repo entity =
            await Client.GetFromJsonAsync<AndNetwork9.Shared.Storage.Repo>($"api/repo/{EntityId}");
        EditedText = entity.Description.Text;
        entity.Nodes = await Client.GetFromJsonAsync<RepoNode[]>($"api/repo/{EntityId}/nodes");
        entity.ReadRule = await Client.GetFromJsonAsync<AccessRule>($"api/AccessRule/{entity.ReadRuleId}");
        entity.WriteRule = await Client.GetFromJsonAsync<AccessRule>($"api/AccessRule/{entity.WriteRuleId}");

        SelectedNodeIndex = entity.Nodes.IndexOf(entity.Nodes.MaxBy(x => x));
        Console.WriteLine(Entity is null);
        Entity = entity;
    }

    private async void UpdateReadRule(AccessRule rule)
    {
        if (Entity.ReadRuleId == rule.Id) return;
        Entity.ReadRuleId = rule.Id;
        Entity = await (await Client.PutAsJsonAsync($"api/repo/{Entity.Id}", Entity)).Content
            .ReadFromJsonAsync<AndNetwork9.Shared.Storage.Repo>();
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    private async void UpdateWriteRule(AccessRule rule)
    {
        if (Entity.WriteRuleId == rule.Id) return;
        Entity.WriteRuleId = rule.Id;
        Entity = await (await Client.PutAsJsonAsync($"api/repo/{Entity.Id}", Entity)).Content
            .ReadFromJsonAsync<AndNetwork9.Shared.Storage.Repo>();
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    private async Task UpdateDescription()
    {
        Entity.Description = await (await Client.PutAsJsonAsync($"api/comment/{Entity.Description.Id}",
            Entity.Description with
            {
                Text = EditedText,
            })).Content.ReadFromJsonAsync<Comment>();
    }

    private void NodeLoaded()
    {
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }
}