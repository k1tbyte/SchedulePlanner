using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace SchedulePlanner.Converters;

public class BaseBooleanConverter<T>(T trueValue, T falseValue) : IValueConverter
{
    private T True { get; set; } = trueValue;
    private T False { get; set; } = falseValue;

    public virtual object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            null => False,
            bool booleanValue => booleanValue ? True : False,
            int intValue when parameter is null => intValue == 0 ? False : True,
            int intValue when parameter is int param => intValue > param ? True : False,
            _ => True
        };

        //Because object not null
    }

    public virtual object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is T t && EqualityComparer<T>.Default.Equals(t, True);
    }
}