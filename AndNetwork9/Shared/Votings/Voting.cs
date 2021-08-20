using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Utility;

namespace AndNetwork9.Shared.Votings
{
    public record Voting
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public virtual string Description { get; set; } = string.Empty;
        public virtual IList<Comment> Comments { get; set; } = new List<Comment>();

        public int ReadRuleId { get; set; }
        [JsonIgnore]
        public virtual AccessRule ReadRule { get; set; } = null!;
        public int EditRuleId { get; set; }
        [JsonIgnore]
        public virtual AccessRule EditRule { get; set; } = null!;

        public int? ReporterId { get; set; }
        [JsonIgnore]
        public virtual Member? Reporter { get; set; }

        public virtual IList<Vote> Votes { get; set; } = new List<Vote>();
        public bool EditVoteEnabled { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime LastEditTime { get; set; }
        public DateTime? EndTime { get; set; }
        public MemberVote Result { get; set; }

        public bool HasReadAccess(Member member)
        {
            return Votes.Any(x => x.MemberId == member.Id) || ReadRule.HasAccess(member);
        }

        public bool HasWriteAccess(Member member) => Reporter?.Id == member.Id || EditRule.HasAccess(member);
    }
}