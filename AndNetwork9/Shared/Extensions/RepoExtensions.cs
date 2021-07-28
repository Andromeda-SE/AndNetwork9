using System;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Storage;

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

        public static string GetFileExtension(this RepoType type)
        {
            return type switch
            {
                RepoType.None => string.Empty,
                RepoType.Blueprint => ".sbc",
                RepoType.Script => ".cs",
                RepoType.World => ".sbc",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }

        public static string GetFileName(this RepoNode node)
        {
            return $"AND.{node.Repo.Name}.{node.Tag}{node.Repo.Type.GetFileExtension()}";
        }
    }
}