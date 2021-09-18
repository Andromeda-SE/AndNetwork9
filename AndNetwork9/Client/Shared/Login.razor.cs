using System;
using System.Net.Http;
using AndNetwork9.Client.Services;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AndNetwork9.Client.Shared
{
    public partial class Login
    {
        [Inject]
        public AuthStateProvider AuthenticationStateProvider { get; set; } = null!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        public HttpClient Client { get; set; } = null!;

        [Parameter]
        public AuthCredentials Credentials { get; set; } = new AuthCredentials(String.Empty, String.Empty);

        public bool LoginEnabled { get; set; } = true;
        public bool ErrorLogin { get; set; }

        public async void LoginAsync()
        {
            LoginEnabled = false;
            ErrorLogin = !await AuthenticationStateProvider.LoginAsync(Credentials)
                .WaitAsync(TimeSpan.FromSeconds(30)).ConfigureAwait(true);
            if (!ErrorLogin) NavigationManager.NavigateTo("/", true);
            LoginEnabled = true;
            StateHasChanged();
        }
    }
}