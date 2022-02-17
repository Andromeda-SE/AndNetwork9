using System;
using System.Collections.Generic;
using System.Linq;
using And9.Client.Clan.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace And9.Client.Clan.ViewModels;

public class NavigationViewModel : ObservableObject
{
    private Control _content = null!;
    private NavigationViewItem _selectedItem = null!;

    public NavigationViewModel() => SelectedItem = MenuItems.First();

    public IList<NavigationViewItem> MenuItems { get; } = new List<NavigationViewItem>
    {
        new()
        {
            Content = "Участники",
            Icon = new SymbolIcon(Symbol.People),
            Tag = typeof(MembersView),
        },
    };

    public NavigationViewItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value, nameof(SelectedItem)))
                Content = (Control)Ioc.Default.GetService((Type)_selectedItem.Tag)!
                          ?? throw new InvalidOperationException();
        }
    }

    public Control Content
    {
        get => _content;
        set => SetProperty(ref _content, value, nameof(Content));
    }
}