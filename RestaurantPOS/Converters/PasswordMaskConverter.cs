using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace RestaurantPOS.Converters
{
    public class PasswordMaskConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string s && s.Length > 0)
                return new string('*', s.Length);
            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
