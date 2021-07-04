using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Shared.Backend.Elections
{
    public record ElectionVoting
    {
        public int ElectionId { get; set; }

        [JsonIgnore]
        public virtual Election Election { get; set; } = null!;

        public Direction Direction { get; set; }

        public int AgainstAll { get; set; } = 0;
        [JsonIgnore]
        public virtual IList<ElectionsMember> Members { get; set; } = new List<ElectionsMember>();

        public Member? GetWinner()
        {
            ElectionsMember? winner = Members.Where(x => x.Votes is not null).MaxBy(x => x.Votes);
            return winner is not null && AgainstAll <= winner.Votes ? winner.Member : null;
        }
    }
}