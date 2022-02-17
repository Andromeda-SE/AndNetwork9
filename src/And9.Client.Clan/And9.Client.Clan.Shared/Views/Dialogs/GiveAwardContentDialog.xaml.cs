using And9.Client.Clan.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace And9.Client.Clan.Views.Dialogs;

public sealed partial class GiveAwardContentDialog : ContentDialog
{
    public GiveAwardContentDialog(GiveAwardViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}