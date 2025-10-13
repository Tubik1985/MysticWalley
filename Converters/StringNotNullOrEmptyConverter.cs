using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MysticWalley.Converters;

/// <summary>
/// Проверяет, является ли входная строка непустой.
/// Возвращает true, если значение — строка и не пустая.
/// Используется, например, для свойства IsVisible в XAML.
/// </summary>
public sealed class StringNotNullOrEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string s && !string.IsNullOrWhiteSpace(s);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException($"{nameof(StringNotNullOrEmptyConverter)}: обратное преобразование не поддерживается.");
    }
}