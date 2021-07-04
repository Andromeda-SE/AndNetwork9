using System;
using AndNetwork9.Client.Utility;

namespace AndNetwork9.Client.Extensions
{
    public static class EnumExtension
    {
        public static string GetIconStyle(this SortType sortType, bool descending) => sortType switch
        {
            SortType.Default => "sort-down",
            SortType.DefaultReverse => "sort-up",
            SortType.Alphabet => "sort-alpha-down",
            SortType.AlphabetReverse => "sort-alpha-up",
            SortType.Numeric => "sort-numeric-down",
            SortType.NumericReverse => "sort-numeric-up",
            _ => throw new ArgumentOutOfRangeException(nameof(sortType), sortType, null)
        } + (descending ? "-alt" : string.Empty);
    }
}