using System;
using System.Globalization;
using System.Windows.Data;

namespace AppTrd.BaseLib.Converters
{
    public class DoubleToPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is double) == false)
                return value;

            var dValue = (double)value;

            var format = parameter as string ?? "N2";

            if (dValue < 0)
                return dValue.ToString(format);

            if (dValue > 0)
                return "+" + dValue.ToString(format);

            return dValue.ToString(format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}