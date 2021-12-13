using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace AndNetwork9.Client.Pages.Candidate;

[AllowAnonymous]
public partial class CandidateRequestPage
{
    private string _discordInput;

    private string _steamInput;
    private string _vkInput;
    private bool _dataChecked;
    private bool _clanChecked;
    private bool _ruleChecked;
    private bool _sumbitProcessing;
    [Inject]
    public HttpClient Client { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    public CandidateRequest Model { get; set; } = new();

    public bool DataChecked
    {
        get => _dataChecked;
        set
        {
            _dataChecked = value;
            StateHasChanged();
        }
    }

    public bool ClanChecked
    {
        get => _clanChecked;
        set
        {
            _clanChecked = value;
            StateHasChanged();
        }
    }

    public bool RuleChecked
    {
        get => _ruleChecked;
        set
        {
            _ruleChecked = value;
            StateHasChanged();
        }
    }

    public string SteamInput
    {
        get => _steamInput;
        set
        {
            _steamInput = value;
            ProcessSteamInput();
            StateHasChanged();
        }
    }

    public bool SumbitProcessing
    {
        get => _sumbitProcessing;
        set
        {
            _sumbitProcessing = value;
            StateHasChanged();
        }
    }

    public string SteamOutput { get; set; }

    [Required]
    public string DiscordInput
    {
        get => _discordInput;
        set
        {
            _discordInput = value;
            ProcessDiscordInput();
            StateHasChanged();
        }
    }

    public string DiscordOutput { get; set; }

    public string VkInput
    {
        get => _vkInput;
        set
        {
            _vkInput = value;
            ProcessVkInput();
            StateHasChanged();
        }
    }

    public string VkOutput { get; set; }
    public bool SumbitDisabled => !DataChecked 
                                  || !ClanChecked 
                                  || !RuleChecked
                                  || string.IsNullOrWhiteSpace(Model.Nickname)
                                  || Model.SteamId == 0
                                  || Model.DiscordId == 0
                                  || SumbitProcessing;

    private async void ProcessSteamInput()
    {
        try
        {
            HttpResponseMessage response = await Client.PostAsJsonAsync("public/api/candidate/steam", _steamInput)
                .ConfigureAwait(true);
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    SteamOutput = "Неверная ссылка на профиль Steam";
                    return;
                case HttpStatusCode.Conflict:
                    SteamOutput =
                        "В клане уже известен игрок с такими данными. Обратитесь к первому советника клана за подробностями";
                    return;
            }

            ulong value = await response.Content.ReadFromJsonAsync<ulong>();
            Model.SteamId = value;
            SteamOutput = $"Указан профиль Steam со следующим ID: {Model.SteamId}";
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async void ProcessDiscordInput()
    {
        try
        {
            HttpResponseMessage response = await Client.PostAsJsonAsync("public/api/candidate/discord", _discordInput)
                .ConfigureAwait(false);
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    DiscordOutput =
                        "Профиль не найден. Убедитесь, что являетесь участником сервера Discord клана: https://discord.gg/3JJ7fdr";
                    return;
                case HttpStatusCode.Conflict:
                    DiscordOutput =
                        "В клане уже известен игрок с такими данными. Обратитесь к первому советника клана за подробностями";
                    return;
            }

            ulong value = await response.Content.ReadFromJsonAsync<ulong>();
            Model.DiscordId = value;
            DiscordOutput = $"Указан профиль Discord со следующим ID: {Model.DiscordId}";
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async void ProcessVkInput()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_vkInput))
            {
                VkOutput = string.Empty;
                return;
            }

            HttpResponseMessage response =
                await Client.PostAsJsonAsync("public/api/candidate/vk", _vkInput).ConfigureAwait(false);
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    VkOutput = "Неверная ссылка на профиль ВК";
                    return;
                case HttpStatusCode.Conflict:
                    VkOutput =
                        "В клане уже известен игрок с такими данными. Обратитесь к первому советника клана за подробностями";
                    return;
            }

            long value = await response.Content.ReadFromJsonAsync<long>();
            Model.VkId = value;
            VkOutput = $"Указан профиль ВК со следующим ID: {Model.VkId}";
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async Task ValidSumbit(EditContext obj)
    {
        SumbitProcessing = true;
        HttpResponseMessage response =
            await Client.PostAsJsonAsync("public/api/candidate/", Model).ConfigureAwait(false);

        SumbitProcessing = false;
    }
}