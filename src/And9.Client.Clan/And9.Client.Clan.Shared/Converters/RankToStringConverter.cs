using System;
using And9.Service.Core.Abstractions;
using And9.Service.Core.Abstractions.Enums;
using Microsoft.UI.Xaml.Data;

namespace And9.Client.Clan.Converters;

public class RankToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is null) return "—";
        if (value is not Rank rank) throw new ArgumentException("value is not Rank", nameof(value));
        return rank.GetDisplayString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}