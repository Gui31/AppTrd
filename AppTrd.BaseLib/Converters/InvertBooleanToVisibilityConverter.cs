using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AppTrd.BaseLib.Converters
{
    public class InvertBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool ? (!(bool)value ? Visibility.Visible : Visibility.Collapsed) : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}