using System;
using Microsoft.UI.Xaml.Data;

namespace And9.Client.Clan.Converters;

public class TimeZoneConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, string language)
    {
        if (value is null) return "—";
        if (value is not TimeZoneInfo timeZoneInfo) throw new ArgumentException("value is not TimeZoneInfo", nameof(value));
        return $"UTC{(timeZoneInfo.BaseUtcOffset >= TimeSpan.Zero ? '+' : '-')}{timeZoneInfo.GetUtcOffset(DateTime.UtcNow):hh\\:mm}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}