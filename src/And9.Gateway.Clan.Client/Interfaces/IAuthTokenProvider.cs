namespace And9.Gateway.Clan.Client.Interfaces;

public interface IAuthTokenProvider
{
    public ValueTask<string> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
    public event Func<string, Task> TokenChanged;
    public ValueTask<string?> GetToken();
    public Task LogoutAsync();
}