using System;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System.Globalization;
using System.IO;

namespace RestaurantPOS.Converters
{
    public class ImageBrushConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string path && File.Exists(path))
                return new ImageBrush(new Bitmap(path)) { Stretch = Stretch.UniformToFill };

            return new ImageBrush { Stretch = Stretch.UniformToFill };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
