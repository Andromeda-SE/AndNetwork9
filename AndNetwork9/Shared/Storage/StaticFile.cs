using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Interfaces;
using AndNetwork9.Shared.Utility;

namespace AndNetwork9.Shared.Storage;

public record StaticFile : IId
{
    public string Name { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;

    public int? OwnerId { get; set; }
    [JsonIgnore]
    public virtual Member? Owner { get; set; }

    public int ReadRuleId { get; set; }
    [JsonIgnore]
    public virtual AccessRule ReadRule { get; set; } = null!;
    [JsonIgnore]
    public virtual IList<Task> Tasks { get; set; } = null!;
    public int Id { get; set; }
    public Guid ConcurrencyToken { get; set; }


    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Extension) ? Name : $"{Name}.{Extension}";
    }

    public DateTime LastChanged { get; set; }
}