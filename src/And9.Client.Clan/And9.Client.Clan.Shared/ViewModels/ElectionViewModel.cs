using And9.Gateway.Clan.Client.Connections;
using And9.Service.Election.Abstractions.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace And9.Client.Clan.ViewModels;

public class ElectionViewModel : ObservableObject
{
    public ElectionViewModel(ElectionConnection electionConnection)
    {
        ElectionConnection = electionConnection;
    }
    private readonly ElectionConnection ElectionConnection;
}