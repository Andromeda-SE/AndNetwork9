using System;
using System.Net.Http;
using AndNetwork9.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AndNetwork9.Client.Shared
{
    public partial class Login
    {
        private string _nickname;
        private string _password;
        [Inject]
        public AuthStateProvider AuthenticationStateProvider { get; set; } = null!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        public HttpClient Client { get; set; } = null!;

        [Parameter]
        public string Nickname
        {
            get => _nickname;
            set
            {
                _nickname = value;
                StateHasChanged();
            }
        }

        [Parameter]
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                StateHasChanged();
            }
        }

        public bool LoginEnabled { get; set; } = true;
        public bool ErrorLogin { get; set; }

        public async void LoginAsync()
        {
            LoginEnabled = false;
            ErrorLogin = !await AuthenticationStateProvider.LoginAsync(new(Nickname, Password))
                .WaitAsync(TimeSpan.FromSeconds(30)).ConfigureAwait(true);
            if (!ErrorLogin) NavigationManager.NavigateTo("/", true);
            LoginEnabled = true;
            StateHasChanged();
        }

        private void Callback(KeyboardEventArgs e)
        {
            if (e.Code == "Enter") LoginAsync();
        }
    }
}