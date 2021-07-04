using System;
using System.Net;

namespace AndNetwork9.Shared.Backend.Auth
{
    public record AuthSession
    {
        public Guid SessionId { get; set; }
        public virtual Member Member { get; set; } = null!;
        public IPAddress Address { get; set; } = IPAddress.None;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        public DateTime? ExpireTime { get; set; }

        public string? Code { get; set; }
        public DateTime CodeExpireTime { get; set; }
    }
}