using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ipsae.Converter;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var boolValue = value is true;
        if (parameter is string param && param == "invert")
            boolValue = !boolValue;
        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var visibility = value is Visibility v && v == Visibility.Visible;
        if (parameter is string param && param == "invert")
            visibility = !visibility;
        return visibility;
    }
}
