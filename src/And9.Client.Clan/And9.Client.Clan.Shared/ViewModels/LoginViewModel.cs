using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using And9.Client.Clan.Views;
using And9.Client.Clan.Views.Pages;
using And9.Gateway.Clan.Client.Connections;
using And9.Service.Award.Abstractions.Models;
using And9.Service.Core.Abstractions.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using WinUIEx;

namespace And9.Client.Clan.ViewModels;

public class LoginViewModel : ObservableObject
{
    private readonly AuthConnection _authConnection;
    private string _errorString = string.Empty;

    private string _login = string.Empty;
    private bool _loginAvailable = true;
    private string _password = string.Empty;
    private bool _persist;

    public LoginViewModel(AuthConnection authConnection)
    {
        _authConnection = authConnection;
        LoginCommand = new AsyncRelayCommand(LoginCommandExec, LoginCommandCanExec);
        CopyVersionCommand = new AsyncRelayCommand(CopyVersionExec);
        IConfiguration config = Ioc.Default.GetRequiredService<IConfiguration>();
        Login = config["LOGIN"];
        Password = config["PASSWORD"];
    }

    public string Login
    {
        get => _login;
        set
        {
            if (SetProperty(ref _login, value, nameof(Login))) LoginCommand.NotifyCanExecuteChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value, nameof(Password))) LoginCommand.NotifyCanExecuteChanged();
        }
    }

    public bool Persist
    {
        get => _persist;
        set => SetProperty(ref _persist, value, nameof(Persist));
    }

    public string ErrorString
    {
        get => _errorString;
        set
        {
            if (SetProperty(ref _errorString, value, nameof(ErrorString))) OnPropertyChanged(nameof(ErrorVisible));
        }
    }

    public bool LoginAvailable
    {
        get => _loginAvailable;
        set
        {
            if (SetProperty(ref _loginAvailable, value, nameof(LoginAvailable))) LoginCommand.NotifyCanExecuteChanged();
        }
    }

    public string? Build => typeof(LoginViewModel).Assembly.GetName().Version?.ToString();
    public Visibility ErrorVisible => string.IsNullOrEmpty(ErrorString) ? Visibility.Collapsed : Visibility.Visible;

    public IRelayCommand LoginCommand { get; }
    public IRelayCommand CopyVersionCommand { get; }

    private bool LoginCommandCanExec() => LoginAvailable && !(string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password));

    private async Task LoginCommandExec(CancellationToken cancellationToken = default)
    {
        //TODO: реализовать проверку возможности подключения к бэку до нажатия кнопки
        LoginAvailable = false;
        ErrorString = string.Empty;
        try
        {
            if (_authConnection.Connection.State == HubConnectionState.Disconnected)
                await _authConnection.Connection.StartAsync(cancellationToken);
            ErrorString = await _authConnection.LoginAsync(Login, Password, cancellationToken);
            if (string.IsNullOrEmpty(ErrorString))
            {
                if (Persist)
                {
                    IConfiguration config = Ioc.Default.GetRequiredService<IConfiguration>();
                    config["LOGIN"] = Login;
                    config["PASSWORD"] = Password;
                }

                WindowEx window = Ioc.Default.GetRequiredService<WindowEx>();
                Frame frame = (Frame)window.Content;

                await Ioc.Default.GetRequiredService<IHost>().StartAsync(cancellationToken);
                await Task.WhenAll(
                    Ioc.Default.GetRequiredService<RepositoryConnection<Member>>().LoadAsync(),
                    Ioc.Default.GetRequiredService<RepositoryConnection<Award>>().LoadAsync());
                frame.Navigate(typeof(NavigationPage), default, new SlideNavigationTransitionInfo());
            }
        }
        catch (Exception e)
        {
            ErrorString = e.Message + Environment.NewLine + e.StackTrace;
            //await Ioc.Default.GetRequiredService<IHost>().StopAsync(cancellationToken);
#if DEBUG
            throw;
#endif
        }
        finally
        {
            LoginAvailable = true;
        }
    }

    private Task CopyVersionExec(CancellationToken cancellationToken = default)
    {
        DataPackage package = new()
        {
            RequestedOperation = DataPackageOperation.Copy,
        };
        package.SetText($"Версия клиента AndromedaNet: {Build}");
        Clipboard.SetContent(package);
        return Task.CompletedTask;
    }
}