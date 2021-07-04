using System;
using System.Text.Json.Serialization;

namespace AndNetwork9.Shared.Storage
{
    public record RepoNode : IComparable<RepoNode>
    {
        public int RepoId { get; set; }
        public int Version { get; set; }
        public int Modification { get; set; }
        public int Prototype { get; set; }

        public int? AuthorId { get; set; }
        [JsonIgnore]
        public virtual Member? Author { get; set; }
        public DateTime CreateTime { get; set; }

        [JsonIgnore]
        public virtual Repo Repo { get; set; } = null!;

        [JsonIgnore]
        public string Tag
        {
            get
            {
                string result = string.Empty;

                if (Version > 0) result += Version.ToString("D");
                if (Modification > 0) result += "M" + Modification.ToString("D");
                if (Prototype > 0) result += "P" + Prototype.ToString("D");

                return result;
            }
        }

        public string Description { get; set; } = string.Empty;

        public int CompareTo(RepoNode? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            int versionComparison = Version.CompareTo(other.Version);
            if (versionComparison != 0) return versionComparison;
            int modificationComparison = Modification.CompareTo(other.Modification);
            if (modificationComparison != 0) return modificationComparison;
            return Prototype.CompareTo(other.Prototype);
        }
    }
}