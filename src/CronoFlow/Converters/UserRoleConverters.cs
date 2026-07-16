using System.Globalization;
using Avalonia.Data.Converters;
using CronoFlow.Models;

namespace CronoFlow.Converters;

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

/// <summary>
/// Bind as MultiBinding with [PendingDeleteTaskId, task.Id] to know whether this
/// specific row is the one currently armed for delete confirmation.
/// </summary>
public class TaskPendingDeleteConverter : IMultiValueConverter
{
    public static readonly TaskPendingDeleteConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var isMatch = values.Count == 2 && values[0] is int pendingId && values[1] is int rowId && pendingId == rowId;
        // ConverterParameter "invert" is used for the non-armed row button (shows "Excluir").
        return parameter as string == "invert" ? !isMatch : isMatch;
    }
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

public class TimerActionTypeLabelConverter : IValueConverter
{
    public static readonly TimerActionTypeLabelConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is TimerActionType action
            ? action switch
            {
                TimerActionType.Start => "Iniciou",
                TimerActionType.Pause => "Pausou",
                TimerActionType.Resume => "Retomou",
                TimerActionType.Stop => "Parou",
                _ => value.ToString()
            }
            : value?.ToString();

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}