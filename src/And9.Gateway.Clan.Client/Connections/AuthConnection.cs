using And9.Gateway.Clan.Client.Interfaces;
using And9.Service.Auth.API.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace And9.Gateway.Clan.Client.Connections;

public class AuthConnection : IClientConnection, IAuthServerMethods, IAuthTokenProvider
{
    private Func<Task<string?>>? _getNewToken;
    private string _token = string.Empty;
    private DateTime _expireTime = DateTime.MinValue;

    public AuthConnection(HubConnection connection) => Connection = connection;

    public bool IsAuthenticated { get; protected set; }

    protected string Token
    {
        get => _token;
        set
        {
            _expireTime = string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.UtcNow.AddMinutes(60);
            _token = value;
            TokenChanged?.Invoke(value);
        }
    }

    public async Task<string?> Login(string username, string password)
    {
        string? result = await Connection.InvokeAsync<string?>(nameof(Login), username, password).ConfigureAwait(false);
        Token = result ?? string.Empty;
        return result;
    }

    public async Task Logout() => await Connection.InvokeAsync(nameof(Logout)).ConfigureAwait(false);
    public async Task AllLogout() => await Connection.InvokeAsync(nameof(AllLogout)).ConfigureAwait(false);
    public async Task GeneratePassword() => await Connection.InvokeAsync(nameof(GeneratePassword)).ConfigureAwait(false);
    public async Task SetPassword(string password) => await Connection.InvokeAsync(nameof(SetPassword), password).ConfigureAwait(false);

    public async ValueTask<string> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        string? token;
        try
        {
            token = await Login(username, password).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            IsAuthenticated = false;
            return e.Message;
        }

        IsAuthenticated = !string.IsNullOrEmpty(token);
        if (!IsAuthenticated) return "Неверный логин или пароль";
        Token = token!;
        _getNewToken = async () => await LoginAsync(username, password).ConfigureAwait(false);
        return string.Empty;
    }

    public event Func<string, Task>? TokenChanged;

    public async ValueTask<string?> GetToken()
    {
        if (_getNewToken is not null && _expireTime <= DateTime.UtcNow) IsAuthenticated = !string.IsNullOrEmpty(await _getNewToken().ConfigureAwait(false));
        return Token;
    }

    public async Task LogoutAsync()
    {
        await Logout().ConfigureAwait(false);
        Token = string.Empty;
        _getNewToken = null;
    }

    public HubConnection Connection { get; }
}