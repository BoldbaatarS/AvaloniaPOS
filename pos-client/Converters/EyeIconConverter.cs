using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace RestaurantPOS.Converters;

public class EyeIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value is bool b && b) 
            ? Geometry.Parse("M1,5 C3,9 7,13 12,13 C17,13 21,9 23,5 C21,1 17,-3 12,-3 C7,-3 3,1 1,5 Z") 
            : Geometry.Parse("M1,1 L23,23 M1,23 L23,1");
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}