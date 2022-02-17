using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using And9.Gateway.Clan.Client.Connections;
using And9.Service.Award.Abstractions;
using And9.Service.Award.Abstractions.Enums;
using And9.Service.Award.Abstractions.Models;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace And9.Client.Clan.ViewModels;

public class GiveAwardViewModel : ObservableObject
{
    private readonly RepositoryConnection<Award> _awards;
    private readonly CoreConnection _coreConnection;
    private string _description = string.Empty;
    private Member[] _members = Array.Empty<Member>();
    private AwardType _selectedAwardType = AwardType.Copper;

    public GiveAwardViewModel(RepositoryConnection<Award> awards, CoreConnection coreConnection)
    {
        _coreConnection = coreConnection;
        _awards = awards;
        GiveCommand = new AsyncRelayCommand(GiveExec, CanExecuteExec);
    }

    public bool CanExecute => CanExecuteExec();

    public Member[] Members
    {
        get => _members;
        set
        {
            if (SetProperty(ref _members, value, nameof(Members)))
            {
                GiveCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(CanExecute));
            }
        }
    }

    public AwardType SelectedAwardType
    {
        get => _selectedAwardType;
        set
        {
            if (SetProperty(ref _selectedAwardType, value, nameof(SelectedAwardType)))
            {
                GiveCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(CanExecute));
            }
        }
    }

    public Tuple<AwardType, string> SelectedAwardTypeTuple
    {
        get => new(SelectedAwardType, SelectedAwardType.GetDisplayString());
        set => SelectedAwardType = value.Item1;
    }

    public Tuple<AwardType, string>[] AllowedAwardTypes { get; private set; }

    public string Description
    {
        get => _description;
        set
        {
            if (SetProperty(ref _description, value, nameof(Description)))
            {
                GiveCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(CanExecute));
            }
        }
    }

    public IAsyncRelayCommand GiveCommand { get; set; }
    public string MembersText => string.Join(", ", Members.Select(x => x.ToString()));

    public async Task Reset(IEnumerable<Member> members)
    {
        Members = members.ToArray();
        Member? me = await _coreConnection.ReadMe();
        if (me is null) throw new();
        switch (me.Rank)
        {
            case Rank.FirstAdvisor:
                AllowedAwardTypes = Enum.GetValues<AwardType>()
                    .Where(x => x != AwardType.None)
                    .OrderBy(x => x)
                    .Select(x => new Tuple<AwardType, string>(x, x.GetDisplayString()))
                    .ToArray();
                break;
            case Rank.Advisor:
                AllowedAwardTypes = new[]
                {
                    AwardType.Copper,
                    AwardType.Bronze,
                }.OrderBy(x => x).Select(x => new Tuple<AwardType, string>(x, x.GetDisplayString())).ToArray();
                break;
            default:
            {
                if (me.IsSquadCommander && me.SquadPartNumber == 0)
                {
                    AllowedAwardTypes = new[]
                    {
                        AwardType.Copper,
                    }.OrderBy(x => x).Select(x => new Tuple<AwardType, string>(x, x.GetDisplayString())).ToArray();
                    ;
                }
                else
                {
                    throw new();
                }

                break;
            }
        }

        OnPropertyChanged(nameof(AllowedAwardTypes));
        SelectedAwardType = AwardType.Copper;
        Description = string.Empty;
    }

    private bool CanExecuteExec() =>
        Members is not null
        && AllowedAwardTypes is not null
        && Members.Any()
        && AllowedAwardTypes.Any(x => x.Item1 == SelectedAwardType)
        && !string.IsNullOrWhiteSpace(Description);

    private async Task GiveExec()
    {
        Member? me = await _coreConnection.ReadMe();
        if (me is null) throw new();
        foreach (Member member in Members)
            _awards.Add(new()
            {
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Description = Description,
                GaveById = me.Id,
                MemberId = member.Id,
                Type = SelectedAwardType,
            });
    }
}