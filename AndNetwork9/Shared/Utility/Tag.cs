using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AndNetwork9.Shared.Utility
{
    public record Tag : IEquatable<Tag?>
    {
        [JsonIgnore]
        public virtual IList<Task> Tasks { get; set; } = new List<Task>(0);
        public string Name { get; init; } = string.Empty;

        public virtual bool Equals(Tag? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}