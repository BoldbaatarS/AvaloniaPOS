using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace RestaurantPOS.Converters
{
    public class AddOffsetConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double num && double.TryParse(parameter?.ToString(), out var offset))
                return num + offset;
            return value ?? 0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
