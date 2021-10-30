using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Management;

public partial class SendMessage
{
    private string _text = string.Empty;
    [Inject]
    public HttpClient Client { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    [Parameter]
    public List<Member> SelectedMembers { get; set; } = new();

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            StateHasChanged();
        }
    }

    private void UpdateMembersList(IReadOnlyCollection<Member> members)
    {
        SelectedMembers = members.ToList();
        StateHasChanged();
    }

    private bool Validate()
    {
        return SelectedMembers.Any() && !string.IsNullOrWhiteSpace(Text) && Text.Length <= 2000;
    }

    private async void Send()
    {
        HttpResponseMessage result = await Client.PostAsJsonAsync("api/send",
            new MessageSendArgs(SelectedMembers.Select(x => x.Id).ToArray(), Text));
        if (result.IsSuccessStatusCode) NavigationManager.NavigateTo("/management/message");
    }
}