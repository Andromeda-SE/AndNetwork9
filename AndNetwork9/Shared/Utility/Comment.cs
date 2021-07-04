using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Storage;

namespace AndNetwork9.Shared.Utility
{
    public record Comment
    {
        public int Id { get; set; }

        public int? AuthorId { get; set; }
        [JsonIgnore]
        public virtual Member? Author { get; set; }
        public string Text { get; set; } = string.Empty;
        public virtual IList<StaticFile> Files { get; set; } = Array.Empty<StaticFile>();

        public DateTime CreateTime { get; set; }
        public DateTime? LastEditTime { get; set; }

        public int? ParentId { get; set; }
        [JsonIgnore]
        public virtual Comment? Parent { get; set; }
        public virtual IList<Comment> Children { get; set; } = Array.Empty<Comment>();

        public Comment? FindComment(int id)
        {
            return Id == id
                ? this
                : Children.Select(child => child.FindComment(id)).FirstOrDefault(result => result is not null);
        }
    }
}