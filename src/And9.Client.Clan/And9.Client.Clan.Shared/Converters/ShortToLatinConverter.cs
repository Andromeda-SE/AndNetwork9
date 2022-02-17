using System;
using System.Text;
using Microsoft.UI.Xaml.Data;

namespace And9.Client.Clan.Converters;

public class IntToLatinConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, string language)
    {
        if (value is null) return "—";
        if (value is not short number) throw new ArgumentException("value is not short", nameof(value));
        if (number < 0 || number > 3999)
            throw new ArgumentOutOfRangeException(nameof(number), "Min = 0, Max = 3999");
        if (number == 0) return "N";
        int rem = number;
        StringBuilder text = new();

        text.Append('M', Math.DivRem(rem, 1000, out rem));
        text.Append('D', Math.DivRem(rem, 500, out rem));
        text.Append('C', Math.DivRem(rem, 100, out rem));
        text.Append('L', Math.DivRem(rem, 50, out rem));
        text.Append('X', Math.DivRem(rem, 10, out rem));
        text.Append('V', Math.DivRem(rem, 5, out rem));
        text.Append('I', rem);

        text.Replace("IIII", "IV");
        text.Replace("VIV", "IX");
        text.Replace("XXXX", "XL");
        text.Replace("LXL", "XC");
        text.Replace("CCCC", "CD");
        text.Replace("DCD", "CM");

        return text.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}