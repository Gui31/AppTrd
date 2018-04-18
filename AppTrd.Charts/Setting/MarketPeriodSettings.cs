using AppTrd.BaseLib.Service;

namespace AppTrd.Charts.Setting
{
    public class MarketPeriodSettings
    {
        public PeriodMode Mode { get; set; }

        public ChartPeriods TimePeriod { get; set; }

        public int TickCount { get; set; }

        public int AverageOpen { get; set; }

        public int MaxSeconds { get; set; }
    }
}