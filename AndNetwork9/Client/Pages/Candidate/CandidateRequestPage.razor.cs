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
                    SteamOutput = "�������� ������ �� ������� Steam";
                    return;
                case HttpStatusCode.Conflict:
                    SteamOutput =
                        "� ����� ��� �������� ����� � ������ �������. ���������� � ������� ��������� ����� �� �������������";
                    return;
            }

            ulong value = await response.Content.ReadFromJsonAsync<ulong>();
            Model.SteamId = value;
            SteamOutput = $"������ ������� Steam �� ��������� ID: {Model.SteamId}";
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
                        "������� �� ������. ���������, ��� ��������� ���������� ������� Discord �����: https://discord.gg/3JJ7fdr";
                    return;
                case HttpStatusCode.Conflict:
                    DiscordOutput =
                        "� ����� ��� �������� ����� � ������ �������. ���������� � ������� ��������� ����� �� �������������";
                    return;
            }

            ulong value = await response.Content.ReadFromJsonAsync<ulong>();
            Model.DiscordId = value;
            DiscordOutput = $"������ ������� Discord �� ��������� ID: {Model.DiscordId}";
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
                    VkOutput = "�������� ������ �� ������� ��";
                    return;
                case HttpStatusCode.Conflict:
                    VkOutput =
                        "� ����� ��� �������� ����� � ������ �������. ���������� � ������� ��������� ����� �� �������������";
                    return;
            }

            long value = await response.Content.ReadFromJsonAsync<long>();
            Model.VkId = value;
            VkOutput = $"������ ������� �� �� ��������� ID: {Model.VkId}";
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