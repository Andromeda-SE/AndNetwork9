using System;
using System.Collections.Generic;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Interfaces;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Shared
{
    public partial class FastMemberSelector
    {
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public IReadOnlyCollection<Member> InitialMembers { get; set; }
        public IList<Member> Members { get; set; }
        [Parameter]
        public IReadOnlyCollection<Squad> Squads { get; set; }
        public FastEditType SelectedEditType { get; set; }
        [Parameter]
        public Action<IReadOnlyCollection<Member>> MembersUpdated { get; set; }
        public enum FastEditType
        {
            One,
            Squad,
            Direction,
            VoteMembers,
            VoteVoted,
            SquadCommanders,
            Advisors,
            All,
        }

        private void Update() => MembersUpdated((IReadOnlyCollection<Member>)Members);
    }

    public interface IFastMemberSelectorEditType
    {
        public IId SelectedEntity { get; }
        public IReadOnlyCollection<IId> Entities { get; }
        public IEnumerable<Member> Members();

    }
}