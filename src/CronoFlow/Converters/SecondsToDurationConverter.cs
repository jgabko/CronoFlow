using System.Globalization;
using Avalonia.Data.Converters;
using CronoFlow.Services;

namespace CronoFlow.Converters;

public class SecondsToDurationConverter : IValueConverter
{
    public static readonly SecondsToDurationConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is long seconds)
            return TimeFormatter.FormatDuration(seconds);
        return "00:00";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
