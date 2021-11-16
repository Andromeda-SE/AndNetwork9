using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Interfaces;
using AndNetwork9.Shared.Storage;
using AndNetwork9.Shared.Votings;

namespace AndNetwork9.Shared.Utility;

public record Comment : IId
{
    public int? AuthorId { get; set; }
    [JsonIgnore]
    public virtual Member? Author { get; set; }
    public string Text { get; set; } = string.Empty;
    //public virtual IList<StaticFile> Files { get; set; } = Array.Empty<StaticFile>();
    [JsonIgnore]
    public int? TaskDescriptionId { get; set; }
    [JsonIgnore]
    public virtual Task? TaskDescription { get; set; }
    [JsonIgnore]
    public int? TaskId { get; set; }
    [JsonIgnore]
    public virtual Task? Task { get; set; }
    [JsonIgnore]
    public int? RepoId { get; set; }
    [JsonIgnore]
    public virtual Repo? Repo { get; set; }
    [JsonIgnore]
    public int? VotingId { get; set; }
    [JsonIgnore]
    public virtual Voting? Voting { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime? LastEditTime { get; set; }

    public int? ParentId { get; set; }
    [JsonIgnore]
    public virtual Comment? Parent { get; set; }
    public virtual IList<Comment> Children { get; set; } = Array.Empty<Comment>();
    public int Id { get; set; }
    public Guid ConcurrencyToken { get; set; }

    public Comment? FindComment(int id)
    {
        return Id == id
            ? this
            : Children.Select(child => child.FindComment(id)).FirstOrDefault(result => result is not null);
    }

    public override string ToString()
    {
        return Text;
    }

    public DateTime LastChanged { get; set; }
}