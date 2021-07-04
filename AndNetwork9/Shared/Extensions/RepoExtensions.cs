using System;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Shared.Extensions
{
    public static class RepoExtensions
    {
        public static string GetContentType(this RepoType type)
        {
            return type switch
            {
                RepoType.None => string.Empty,
                RepoType.Blueprint => "application/xml",
                RepoType.Script => "text/plain",
                RepoType.World => "application/xml",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }

        public static string GetFileName(this RepoType type)
        {
            return type switch
            {
                RepoType.None => string.Empty,
                RepoType.Blueprint => "bp.sbc",
                RepoType.Script => "Scirpt.cs",
                RepoType.World => "Sandbox.sbc",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }
    }
}