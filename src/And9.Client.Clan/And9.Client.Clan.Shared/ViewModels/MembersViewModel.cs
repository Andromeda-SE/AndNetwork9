using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using And9.Client.Clan.Views;
using And9.Client.Clan.Views.Dialogs;
using And9.Gateway.Clan.Client.Connections;
using And9.Service.Award.Abstractions.Models;
using And9.Service.Core.Abstractions.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace And9.Client.Clan.ViewModels;

public class MembersViewModel : ObservableObject
{
    private Member? _selectedMember;

    public MembersViewModel(RepositoryConnection<Member> repository, RepositoryConnection<Award> awards)
    {
        Repository = repository;
        Awards = awards;
        GiveAwardCommand = new AsyncRelayCommand(GiveAwardExec, GiveAwardCanExec);
        SelectionChanged = new RelayCommand<SelectionChangedEventArgs>(SelectionChangedExec);
        Awards.CollectionChanged += AwardsOnCollectionChanged;
        Repository.CollectionChanged += RepositoryOnCollectionChanged;
    }

    public RepositoryConnection<Member> Repository { get; }
    public RepositoryConnection<Award> Awards { get; }
    public IRelayCommand<SelectionChangedEventArgs> SelectionChanged { get; }
    public IAsyncRelayCommand GiveAwardCommand { get; }

    public Member? SelectedMember
    {
        get => _selectedMember;
        set
        {
            if (SetProperty(ref _selectedMember, value, nameof(SelectedMember))) OnPropertyChanged(nameof(MemberAwards));
        }
    }

    public List<Member> SelectedMembers { get; } = new();

    public Award[]? MemberAwards => SelectedMember is null ? null : Awards.Where(x => x.MemberId == SelectedMember.Id).ToArray();

    private void SelectionChangedExec(SelectionChangedEventArgs args)
    {
        SelectedMembers.AddRange(args.AddedItems.OfType<Member>());
        foreach (Member member in args.RemovedItems.OfType<Member>()) SelectedMembers.Remove(member);
        GiveAwardCommand.NotifyCanExecuteChanged();
    }

    private void AwardsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(MemberAwards));
    }

    private void RepositoryOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Repository));
    }

    ~MembersViewModel()
    {
        Awards.CollectionChanged -= AwardsOnCollectionChanged;
    }

    private bool GiveAwardCanExec() => SelectedMembers.Any();

    private async Task GiveAwardExec()
    {
        try
        {
            GiveAwardContentDialog dialog = Ioc.Default.GetRequiredService<GiveAwardContentDialog>();
            await ((GiveAwardViewModel)dialog.DataContext).Reset(SelectedMembers);
            dialog.XamlRoot = Ioc.Default.GetRequiredService<MembersView>().XamlRoot;
            await dialog.ShowAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}