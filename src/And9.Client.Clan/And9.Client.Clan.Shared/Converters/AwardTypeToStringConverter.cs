using System;
using And9.Service.Award.Abstractions;
using And9.Service.Award.Abstractions.Enums;
using Microsoft.UI.Xaml.Data;

namespace And9.Client.Clan.Converters;

public class AwardTypeToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, string language)
    {
        if (value is null) return "—";
        if (value is not AwardType awardType) throw new ArgumentException("value is not AwardType", nameof(value));
        return awardType.GetDisplayString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}