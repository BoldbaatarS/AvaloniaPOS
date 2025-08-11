using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace RestaurantPOS.Converters
{
    public class NullToAddEditConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value == null ? "Нэмэх" : "Засах";

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
