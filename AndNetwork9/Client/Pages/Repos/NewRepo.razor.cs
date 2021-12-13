using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AndNetwork9.Client.Services;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Repos;

public partial class NewRepo
{
    [Inject]
    public HttpClient Client { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public string Name { get; set; }
    [Parameter]
    public RepoType Type { get; set; }
    [Parameter]
    public AccessRule ReadRule { get; set; } = new()
    {
        Directions = Enum.GetValues<Direction>().Where(x => x > Direction.None).ToArray(),
        MinRank = Rank.Neophyte,
        Squad = null,
        SquadId = null,
    };
    [Parameter]
    public AccessRule WriteRule { get; set; } = new()
    {
        Directions = Enum.GetValues<Direction>().Where(x => x > Direction.None).ToArray(),
        MinRank = Rank.Advisor,
        Squad = null,
        SquadId = null,
    };

    private async Task CreateRepo()
    {
        HttpResponseMessage readResponse = await Client.PostAsJsonAsync("api/AccessRule", ReadRule);
        HttpResponseMessage writeResponse = await Client.PostAsJsonAsync("api/AccessRule", WriteRule);
        AccessRule readRule = await readResponse.Content.ReadFromJsonAsync<AccessRule>();
        AccessRule writeRule = await writeResponse.Content.ReadFromJsonAsync<AccessRule>();

        HttpResponseMessage repoResponse = await Client.PostAsJsonAsync("api/repo",
            new AndNetwork9.Shared.Storage.Repo
            {
                ReadRuleId = readRule.Id,
                ReadRule = readRule,
                WriteRuleId = writeRule.Id,
                WriteRule = writeRule,
                CreatorId = AuthStateProvider.CurrentMember.Id,
                Creator = AuthStateProvider.CurrentMember,
                Name = Name,
                Type = Type,
                RepoName = Name,
            });
        if (repoResponse.IsSuccessStatusCode)
        {
            AndNetwork9.Shared.Storage.Repo repo =
                await repoResponse.Content.ReadFromJsonAsync<AndNetwork9.Shared.Storage.Repo>();
            NavigationManager.NavigateTo($"repo/{repo.Id}");
        }
    }
}