using Avalonia.Data.Converters;
using System;
using System.Globalization;
using System.Collections.Generic;

namespace RestaurantPOS.Converters
{
    public class MaskedTextConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            var text = values[0] as string ?? string.Empty;
            var isVisible = values[1] is bool b && b;
            return isVisible ? text : new string('*', text.Length);
        }

        public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            var text = value as string ?? string.Empty;
            return new object[] { text, Avalonia.Data.BindingOperations.DoNothing };
        }
    }
}
