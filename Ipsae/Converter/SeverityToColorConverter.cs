using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Ipsae.Converter;

public class SeverityToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var severity = value as string ?? "";
        var isBackground = parameter is string p && p == "bg";

        return severity.ToUpperInvariant() switch
        {
            "HIGH" => isBackground
                ? new SolidColorBrush(Color.FromRgb(0x33, 0x10, 0x10))
                : new SolidColorBrush(Color.FromRgb(0xCC, 0x55, 0x55)),
            "MEDIUM" => isBackground
                ? new SolidColorBrush(Color.FromRgb(0x33, 0x2A, 0x10))
                : new SolidColorBrush(Color.FromRgb(0xCC, 0xAA, 0x55)),
            _ => isBackground
                ? new SolidColorBrush(Color.FromRgb(0x10, 0x1A, 0x10))
                : new SolidColorBrush(Color.FromRgb(0x55, 0xAA, 0x66)),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
