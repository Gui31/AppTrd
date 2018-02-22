using System;
using System.Collections.Generic;
using System.Globalization;

namespace AppTrd.BaseLib.Common
{
    public class LinearAxis
    {
        public static double CalculateActualInterval(double height, double min, double max, double maxIntervals = 6.0)
        {
            // Percentage
            if (min == 0 && max == 100)
                return 25;

            Func<double, double> funcExponent = (double x) => Math.Ceiling(Math.Log(x, 10.0));
            Func<double, double> funcMantissa = (double x) => x / Math.Pow(10.0, funcExponent(x) - 1.0);
            double maxIntervalCount = height / 200.0 * maxIntervals;
            double range = Math.Abs(min - max);
            double interval = Math.Pow(10.0, funcExponent(range));
            double tempInterval = interval;
            while (true)
            {
                int mantissa = (int)funcMantissa(tempInterval);
                if (mantissa == 5)
                {
                    tempInterval = RemoveNoiseFromDoubleMath(tempInterval / 2.5);
                }
                else
                {
                    if (mantissa == 2 || mantissa == 1 || mantissa == 10)
                    {
                        tempInterval = RemoveNoiseFromDoubleMath(tempInterval / 2.0);
                    }
                }
                if (range / tempInterval > maxIntervalCount)
                {
                    break;
                }
                interval = tempInterval;
            }
            return interval;
        }

        internal static double RemoveNoiseFromDoubleMath(double value)
        {
            if (value == 0.0 || Math.Abs(Math.Log10(Math.Abs(value))) < 27.0)
            {
                return (double)((double)value);
            }
            return double.Parse(value.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }

        public static IEnumerable<double> GetMajorValues(double height, double min, double max, double actualInterval)
        {
            double num = AlignToInterval(min, actualInterval);

            double num2 = num;
            int num3 = 1;
            bool rangeEqual = false;
            while (num2 <= max)
            {
                rangeEqual = num2 == max;
                yield return num2;
                num2 = num + (double)num3 * actualInterval;
                num3++;
            }

            if (max < 0)
            {
                yield return num2;
            }
            else
            {
                if (num2 > max && rangeEqual == false)
                    yield return num2;
            }
            yield break;
        }

        // System.Windows.Controls.DataVisualization.Charting.LinearAxis
        private static double AlignToInterval(double value, double interval)
        {
            double typedInterval = interval;
            double typedValue = value;
            return RemoveNoiseFromDoubleMath(RemoveNoiseFromDoubleMath(Math.Floor(typedValue / typedInterval)) * typedInterval);
        }
    }
}