using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace RestaurantPOS.Converters;

public sealed class PathToBitmapConverter : IValueConverter
{
    public static readonly PathToBitmapConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var path = value as string;
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            return new Bitmap(path);

        var fallback = Path.Combine(AppContext.BaseDirectory, "Assets", "Default", "Hall.png");
        return new Bitmap(fallback);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
