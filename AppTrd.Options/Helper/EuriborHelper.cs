using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AppTrd.BaseLib.Common;

namespace AppTrd.Options.Helper
{
    public static class EuriborHelper
    {
        private const string BaseUrl = "https://www.emmi-benchmarks.eu/assets/modules/rateisblue/file_processing/publication/processed/hist_EURIBOR_{0}.csv";

        private static List<Tuple<TimeSpan, double>> _rates;

        public static void Init()
        {
            var client = new HttpClient();

            var csv = client.GetStringAsync(string.Format(BaseUrl, DateTime.Now.Year)).RunSync();

            LoadCsv(csv);

            // TODO : ajout d'un cache pour le fichier si ok
        }

        public static double GetInterestRate(TimeSpan period)
        {
            if (_rates == null || _rates.Count < 2)
                Init();

            if (_rates == null || _rates.Count < 2)
                return 0;

            if (period <= _rates.First().Item1)
                return _rates.First().Item2;

            if (period >= _rates.Last().Item1)
                return _rates.Last().Item2;

            var low = _rates.Last(r => r.Item1 < period);
            var high = _rates.First(r => r.Item1 >= period);

            if (low.Item2 == high.Item2)
                return low.Item2;

            return (period.TotalDays - low.Item1.TotalDays) / (high.Item1.TotalDays - low.Item1.TotalDays) * (high.Item2 - low.Item2) + low.Item2;
        }

        private static void LoadCsv(string csv)
        {
            var rates = new List<Tuple<TimeSpan, double>>();

            using (var reader = new StringReader(csv))
            {
                var line = reader.ReadLine();

                do
                {
                    line = reader.ReadLine();

                    if (line == null)
                        break;

                    var datas = line.Split(',');

                    rates.Add(new Tuple<TimeSpan, double>(ReadTimeSpan(datas.First()), ReadRate(datas.Last())));

                } while (line != null);
            }

            _rates = rates.OrderBy(r => r.Item1).ToList();
        }

        private static TimeSpan ReadTimeSpan(string period)
        {
            var value = int.Parse(period.Substring(0, period.Length - 1));

            if (period.Last() == 'w')
                return new TimeSpan(7 * value, 0, 0, 0, 0);

            return new TimeSpan(30 * value, 0, 0, 0, 0);
        }

        private static double ReadRate(string str)
        {
            return double.Parse(str, CultureInfo.InvariantCulture) / 100;
        }
    }
}
