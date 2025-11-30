using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace WF2.Converters;

public class CountToBoolConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter,
        CultureInfo culture) =>
        value is int count && parameter is string conditionString &&
        int.TryParse(conditionString, out var condition)
            ? count > condition
            : null;

    public object? ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}