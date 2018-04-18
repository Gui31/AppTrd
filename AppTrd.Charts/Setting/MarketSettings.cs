using AppTrd.BaseLib.Service;
using Microsoft.Practices.ServiceLocation;

namespace AppTrd.Charts.Setting
{
    public class MarketSettings
    {
        public string Epic { get; set; }

        public string Name { get; set; }

        public MarketPeriodSettings TopLeftPeriod { get; set; }

        public MarketPeriodSettings BottomLeftPeriod { get; set; }

        public MarketPeriodSettings RightPeriod { get; set; }

        public double Size { get; set; }

        public bool HasStop { get; set; }

        public int StopDistance { get; set; }

        public bool GarantedStop { get; set; }

        public bool HasLimit { get; set; }

        public int LimitDistance { get; set; }

        public static MarketSettings GetDefault(string epic)
        {
            var tradingService = ServiceLocator.Current.GetInstance<ITradingService>();

            var marketDetails = tradingService.GetMarketDetails(epic);

            return new MarketSettings
            {
                Epic = epic,
                Name = marketDetails.instrument.name,
                HasStop = true,
                GarantedStop = true,
                StopDistance = 20,
                HasLimit = false,
                LimitDistance = 10,
                Size = 1,
                TopLeftPeriod = new MarketPeriodSettings
                {
                    Mode = PeriodMode.Time,
                    TimePeriod = ChartPeriods.OneHour
                },
                BottomLeftPeriod = new MarketPeriodSettings
                {
                    Mode = PeriodMode.Time,
                    TimePeriod = ChartPeriods.FiveMinutes
                },
                RightPeriod = new MarketPeriodSettings
                {
                    Mode = PeriodMode.Time,
                    TimePeriod = ChartPeriods.OneMinute
                }
            };
        }
    }
}