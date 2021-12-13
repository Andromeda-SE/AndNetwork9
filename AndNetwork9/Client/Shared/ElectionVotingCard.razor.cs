using System;
using System.Collections.Generic;
using System.Linq;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Elections;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AndNetwork9.Client.Shared;

public partial class ElectionVotingCard
{
    [Parameter]
    public CouncilElectionVote ElectionVote { get; set; }
    [Parameter]
    public IReadOnlyCollection<Member> AllMembers { get; set; }
    [Parameter]
    public IDictionary<int, uint> Bulletin { get; set; }
    [Parameter]
    public bool ReadOnly { get; set; }
    [Parameter]
    public Action<bool> UpdateAllowSend { get; set; }

    public long MaxVotes
    {
        get { return ElectionVote.Votes.Count(x => x.Key > 0); }
    }

    public long UsedVotes
    {
        get { return Bulletin.Values.Sum(x => x); }
    }

    public long VotesRemaining => MaxVotes - UsedVotes;

    public bool AllowPlus => UsedVotes < MaxVotes && !ReadOnly;

    public bool AllowSend => UsedVotes == MaxVotes || ReadOnly;

    public bool AllowMinus(int memberId) => Bulletin[memberId] > 0 && !ReadOnly;

    public void Add(int memberId, MouseEventArgs e)
    {
        Bulletin[memberId] += e.CtrlKey ? (uint)VotesRemaining : 1;
        UpdateAllowSend(AllowSend);
    }

    public void Remove(int memberId, MouseEventArgs e)
    {
        Bulletin[memberId] -= e.CtrlKey ? Bulletin[memberId] : 1;
        UpdateAllowSend(false);
    }
}