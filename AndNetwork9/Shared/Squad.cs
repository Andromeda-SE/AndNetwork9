using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Converters;
using AndNetwork9.Shared.Extensions;
using AndNetwork9.Shared.Interfaces;

namespace AndNetwork9.Shared
{
    public record Squad : IId
    {
        public int Number { get; set; }
        public string? Name { get; set; }
        [JsonIgnore]
        public ulong? DiscordRoleId { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly CreateDate { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? DisbandDate { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<Task> Tasks => Members.SelectMany(x => x.Tasks);
        [JsonIgnore]
        public virtual Member? Commander => Members.SingleOrDefault(x => x.IsSquadCommander);
        [JsonIgnore]
        public virtual IList<Member> Members { get; set; } = new List<Member>();
        [JsonIgnore]
        public virtual IList<Member> Candidates { get; set; } = new List<Member>();

        public string? Description { get; set; }
        [JsonIgnore]
        public string? Comment { get; set; }

        int IId.Id => Number;

        public override string ToString() =>
            (Name is null ? Number.ToRoman() : Number.ToRoman() + " " + Name) + " отряд";
    }
}