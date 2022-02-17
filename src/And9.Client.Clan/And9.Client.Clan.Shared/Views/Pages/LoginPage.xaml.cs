using And9.Client.Clan.ViewModels;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace And9.Client.Clan.Views.Pages;

public sealed partial class LoginPage : Page
{
    public LoginPage()
    {
        DataContext = Ioc.Default.GetRequiredService<LoginViewModel>();
        InitializeComponent();
    }
}