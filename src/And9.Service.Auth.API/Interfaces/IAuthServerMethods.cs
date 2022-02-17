namespace And9.Service.Auth.API.Interfaces;

public interface IAuthServerMethods
{
    Task<string?> Login(string username, string password);
    Task Logout();
    Task AllLogout();
    Task GeneratePassword();
    Task SetPassword(string password);
}