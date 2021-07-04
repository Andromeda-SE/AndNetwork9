using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Shared.Utility
{
    public record AccessRule : IComparable<AccessRule>
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual Direction[] Directions { get; set; } = Enum.GetValues<Direction>().ToArray();
        public Rank MinRank { get; set; } = Rank.Neophyte;

        public int? SquadId { get; set; }
        [JsonIgnore]
        public virtual Squad? Squad { get; set; }

        [JsonIgnore]
        public virtual IList<Member> AllowedMembers { get; set; } = Array.Empty<Member>();

        public int CompareTo(AccessRule? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            return Id.CompareTo(other.Id);
        }

        public bool HasAccess(Member member)
        {
            Direction memberDirection = member.Direction;
            return AllowedMembers.Any(x => x.Id == member.Id)
                   || Directions.Any(x => x == memberDirection)
                   && member.Rank >= MinRank
                   && (Squad is null || member.Squad == Squad);
        }
    }
}