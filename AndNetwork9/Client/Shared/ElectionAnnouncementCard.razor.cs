using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AndNetwork9.Client.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Elections;
using Microsoft.AspNetCore.Components;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Client.Shared
{
    public partial class ElectionAnnouncementCard
    {
        [Parameter]
        public CouncilElectionVote ElectionVote { get; set; }
        [Parameter]
        public IReadOnlyCollection<Member> AllMembers { get; set; }
        public bool Initialized { get; set; }
        private int[] _winners;
        private string GetColor(int memberId) => memberId != 0 && _winners.Any(x => x == memberId) ? ElectionVote.Direction.GetColorStyle() : "#6c757d";
        private double GetPercent(uint votes)
        {
            double result = votes / (double)ElectionVote.Votes.Values.Sum(x => x);
            Console.WriteLine($"{ElectionVote.Direction}: {votes} / {ElectionVote.Votes.Values.Sum(x => x)} = {result}");
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
}
