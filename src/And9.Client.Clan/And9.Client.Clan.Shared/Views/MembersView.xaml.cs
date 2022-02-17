using And9.Client.Clan.ViewModels;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace And9.Client.Clan.Views;

public sealed partial class MembersView : UserControl
{
    public MembersView()
    {
        DataContext = Ioc.Default.GetRequiredService<MembersViewModel>();
        InitializeComponent();
    }
}