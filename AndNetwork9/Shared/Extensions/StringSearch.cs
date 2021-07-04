using System;
using System.Collections.Generic;
using System.Linq;

namespace AndNetwork9.Shared.Extensions
{
    public static class StringSearch
    {
        public static T? FindByString<T>(this IEnumerable<T?> values, Func<T?, string> select, string value)
            where T : class
        {
            T?[] valuesArray = values.ToArray();

            T? result = valuesArray.FirstOrDefault(x => select(x).Equals(value, StringComparison.CurrentCulture));
            result ??= valuesArray.FirstOrDefault(x =>
                select(x).Equals(value, StringComparison.CurrentCultureIgnoreCase));

            result ??= valuesArray.FirstOrDefault(x => select(x).StartsWith(value, StringComparison.CurrentCulture));
            result ??= valuesArray.FirstOrDefault(x =>
                select(x).StartsWith(value, StringComparison.CurrentCultureIgnoreCase));

            result ??= valuesArray.FirstOrDefault(x => select(x).EndsWith(value, StringComparison.CurrentCulture));
            result ??= valuesArray.FirstOrDefault(x =>
                select(x).EndsWith(value, StringComparison.CurrentCultureIgnoreCase));

            result ??= valuesArray.FirstOrDefault(x => select(x).Contains(value, StringComparison.CurrentCulture));
            result ??= valuesArray.FirstOrDefault(x =>
                select(x).Contains(value, StringComparison.CurrentCultureIgnoreCase));

            return result;
        }
    }
}