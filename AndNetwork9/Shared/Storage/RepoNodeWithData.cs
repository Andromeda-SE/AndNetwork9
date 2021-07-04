using System;

namespace AndNetwork9.Shared.Storage
{
    public record RepoNodeWithData : RepoNode
    {
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}