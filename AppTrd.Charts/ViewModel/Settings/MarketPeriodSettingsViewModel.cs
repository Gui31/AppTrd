using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AppTrd.BaseLib.Service;
using AppTrd.BaseLib.ViewModel;
using AppTrd.Charts.Setting;

namespace AppTrd.Charts.ViewModel.Settings
{
    public class MarketPeriodSettingsViewModel : BaseViewModel
    {
        private readonly MarketPeriodSettings _periodSettings;

        private PeriodMode _mode;
        public PeriodMode Mode
        {
            get { return _mode; }
            set
            {
                if (_mode == value)
                    return;

                _mode = value;
                RaisePropertyChanged(() => Mode);
                RaisePropertyChanged(() => TimeVisibility);
                RaisePropertyChanged(() => TickVisibility);
            }
        }

        public Visibility TimeVisibility
        {
            get { return Mode == PeriodMode.Time ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility TickVisibility
        {
            get { return Mode == PeriodMode.Tick ? Visibility.Visible : Visibility.Collapsed; }
        }

        private ChartPeriods _timePeriod;
        public ChartPeriods TimePeriod
        {
            get { return _timePeriod; }
            set
            {
                if (_timePeriod == value)
                    return;

                _timePeriod = value;
                RaisePropertyChanged(() => TimePeriod);
            }
        }

        private int _tickCount;
        public int TickCount
        {
            get { return _tickCount; }
            set
            {
                if (_tickCount == value)
                    return;

                _tickCount = value;
                RaisePropertyChanged(() => TickCount);
            }
        }

        private int _averageOpen;
        public int AverageOpen
        {
            get { return _averageOpen; }
            set
            {
                if (_averageOpen == value)
                    return;

                _averageOpen = value;
                RaisePropertyChanged(() => AverageOpen);
            }
        }

        private int _maxSeconds;
        public int MaxSeconds
        {
            get { return _maxSeconds; }
            set
            {
                if (_maxSeconds == value)
                    return;

                _maxSeconds = value;
                RaisePropertyChanged(() => MaxSeconds);
            }
        }

        public MarketPeriodSettingsViewModel(MarketPeriodSettings periodSettings)
        {
            _periodSettings = periodSettings;

            Mode = _periodSettings.Mode;
            TimePeriod = _periodSettings.TimePeriod;
            TickCount = _periodSettings.TickCount;
            AverageOpen = _periodSettings.AverageOpen;
            MaxSeconds = _periodSettings.MaxSeconds;
        }

        public MarketPeriodSettings Validate()
        {
            _periodSettings.Mode = Mode;
            _periodSettings.TimePeriod = TimePeriod;
            _periodSettings.TickCount = TickCount;
            _periodSettings.AverageOpen = AverageOpen;
            _periodSettings.MaxSeconds = MaxSeconds;

            return _periodSettings;
        }
    }
}
