using System.Globalization;
using System.Windows.Data;

namespace Engine;

public sealed class AngleToOpacityConverter(double start = 20, double range = 30) : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null) return Binding.DoNothing;
        var angle = (double)value;
        return Math.Clamp((angle - start) / range, 0.0, 1.0);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => Binding.DoNothing;
}