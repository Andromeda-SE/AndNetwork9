using System;
using System.Linq;
using System.Text;

namespace AndNetwork9.Shared.Extensions
{
    public static class RomanExtensions
    {
        public static string ToRoman(this in uint number) => ((int)number).ToRoman();

        public static string ToRoman(this in int number)
        {
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

        public static int FromRoman(this string number)
        {
            if (number is null) throw new ArgumentNullException(nameof(number));
            if (string.Compare(number, "N", StringComparison.InvariantCultureIgnoreCase) == 0) return 0;

            string text = number.ToUpperInvariant().Replace("CM", "DCD", StringComparison.InvariantCultureIgnoreCase)
                .Replace("CD", "CCCC", StringComparison.InvariantCultureIgnoreCase)
                .Replace("XC", "LXL", StringComparison.InvariantCultureIgnoreCase)
                .Replace("XL", "XXXX", StringComparison.InvariantCultureIgnoreCase)
                .Replace("IX", "VIV", StringComparison.InvariantCultureIgnoreCase)
                .Replace("IV", "IIII", StringComparison.InvariantCultureIgnoreCase);

            int result = text.Count(x => x == 'I');
            result += text.Count(x => x == 'V') * 5;
            result += text.Count(x => x == 'X') * 10;
            result += text.Count(x => x == 'L') * 50;
            result += text.Count(x => x == 'C') * 100;
            result += text.Count(x => x == 'D') * 500;
            result += text.Count(x => x == 'M') * 1000;
            return result;
        }
    }
}