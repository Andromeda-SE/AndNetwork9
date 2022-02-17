using System;
using System.Collections.Generic;
using System.Linq;
using AndNetwork9.Client.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Elections;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Shared;

public partial class ElectionAnnouncementCard
{
    private int[] _winners;
    [Parameter]
    public CouncilElectionVote ElectionVote { get; set; }
    [Parameter]
    public IReadOnlyCollection<Member> AllMembers { get; set; }
    public bool Initialized { get; set; }

    private string GetColor(int memberId)
    {
        return memberId != 0 && _winners.Any(x => x == memberId)
            ? ElectionVote.Direction.GetColorStyle()
            : "#6c757d";
    }

    private double GetPercent(uint votes)
    {
        double result = votes / (double)ElectionVote.Votes.Values.Sum(x => x);
        return double.IsNaN(result) ? 0.0 : result;
    }

    protected override void OnInitialized()
    {
        uint maxVote = ElectionVote.Votes.Values.Max();
        IEnumerable<KeyValuePair<int, uint>> rawWinners = ElectionVote.Votes.Where(x => x.Value == maxVote);
        _winners = rawWinners.Where(x => x.Key != 0).Select(x => x.Key).ToArray();
        if (!_winners.Any()) _winners = new[] {0};
        Initialized = true;
    }
}