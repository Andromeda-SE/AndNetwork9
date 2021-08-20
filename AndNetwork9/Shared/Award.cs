using System;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Converters;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Shared
{
    public record Award
    {
        public int Id { get; set; }

        public int MemberId { get; set; }
        [JsonIgnore]
        public virtual Member? Member { get; set; } = null;

        public AwardType Type { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly Date { get; set; }

        public int? GaveById { get; set; }
        [JsonIgnore]
        public virtual Member? GaveBy { get; set; }

        public string? Description { get; set; }
    }
}