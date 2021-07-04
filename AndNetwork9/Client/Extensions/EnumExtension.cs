using System;
using AndNetwork9.Client.Utility;

namespace AndNetwork9.Client.Extensions
{
    public static class EnumExtension
    {
        public static string GetIconStyle(this SortType sortType, bool reverse) => "bi-sort" + sortType switch
        {
            SortType.Default => string.Empty,
            SortType.Alphabet => "-alpha",
            SortType.Numeric => "-numeric",
            _ => throw new ArgumentOutOfRangeException(nameof(sortType), sortType, null)
        } + "-down" + (reverse ? "-alt" : string.Empty);
    }
}