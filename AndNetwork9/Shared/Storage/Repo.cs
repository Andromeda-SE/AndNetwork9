using System.Collections.Generic;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Interfaces;
using AndNetwork9.Shared.Utility;

namespace AndNetwork9.Shared.Storage
{
    public record Repo : IId
    {
        public string Name { get; set; } = string.Empty;
        public string RepoName { get; set; } = string.Empty;
        public RepoType Type { get; set; }
        public int? CreatorId { get; set; }

        public virtual Member? Creator { get; set; }

        [JsonIgnore]
        public virtual IList<RepoNode> Nodes { get; set; } = new List<RepoNode>();

        [JsonIgnore]
        public int CommentId { get; set; }
        public virtual Comment? Description { get; set; }
        public virtual IList<Comment>? Comments { get; set; } = null!;

        public int ReadRuleId { get; set; }
        [JsonIgnore]
        public virtual AccessRule? ReadRule { get; set; } = null!;
        public int WriteRuleId { get; set; }
        [JsonIgnore]
        public virtual AccessRule? WriteRule { get; set; } = null!;
        public int Id { get; set; }
    }
}