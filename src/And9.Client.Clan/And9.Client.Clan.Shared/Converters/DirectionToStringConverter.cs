using System;
using And9.Service.Core.Abstractions;
using And9.Service.Core.Abstractions.Enums;
using Microsoft.UI.Xaml.Data;

namespace And9.Client.Clan.Converters;

public class DirectionToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, string language)
    {
        if (value is null) return "—";
        if (value is not Direction direction) throw new ArgumentException("value is not Direction", nameof(value));
        return direction.GetDisplayString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}