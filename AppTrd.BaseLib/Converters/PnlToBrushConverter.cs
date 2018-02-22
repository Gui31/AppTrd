using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AppTrd.BaseLib.Converters
{
    public class PnlToBrushConverter : IValueConverter
    {
        private SolidColorBrush _underZeroBrush = new SolidColorBrush(Colors.Red);
        private SolidColorBrush _zeroBrush = new SolidColorBrush(Colors.Blue);
        private SolidColorBrush _overZeroBrush = new SolidColorBrush(Colors.LimeGreen);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is double) == false)
                return value;

            var dValue = (double)value;

            if (dValue < 0)
                return _underZeroBrush;

            if (dValue > 0)
                return _overZeroBrush;

            return _zeroBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}