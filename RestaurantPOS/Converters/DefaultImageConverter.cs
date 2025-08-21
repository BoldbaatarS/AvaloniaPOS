using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;

public class DefaultImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var path = value as string;
        return !string.IsNullOrWhiteSpace(path) && File.Exists(path)
            ? path
            : "avares://RestaurantPOS/Assets/Images/default.png";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value!;
}
