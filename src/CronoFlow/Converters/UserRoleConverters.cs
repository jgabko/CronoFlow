using System.Globalization;
using Avalonia.Data.Converters;
using TimeFlow.Models;

namespace TimeFlow.Converters;

public class UserRoleLabelConverter : IValueConverter
{
    public static readonly UserRoleLabelConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is UserRole role)
            return role == UserRole.Manager ? "Gerente" : "Funcionário";
        return value?.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class ActiveStatusConverter : IValueConverter
{
    public static readonly ActiveStatusConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool active)
            return active ? "Ativo" : "Inativo";
        return "—";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class ActiveStatusColorConverter : IValueConverter
{
    public static readonly ActiveStatusColorConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool active)
            return active ? "#66BB6A" : "#EF5350";
        return "#78909C";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
