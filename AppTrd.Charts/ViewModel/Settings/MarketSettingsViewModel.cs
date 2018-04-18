using System;
using AppTrd.BaseLib.ViewModel;
using AppTrd.Charts.Setting;

namespace AppTrd.Charts.ViewModel.Settings
{
    public class MarketSettingsViewModel : BaseViewModel
    {
        private MarketSettings _epicSettings;

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        private double _size;
        public double Size
        {
            get { return _size; }
            set
            {
                if (_size == value)
                    return;

                _size = value;
                RaisePropertyChanged(() => Size);
            }
        }

        private bool _hasStop;
        public bool HasStop
        {
            get { return _hasStop; }
            set
            {
                if (_hasStop == value)
                    return;

                _hasStop = value;
                RaisePropertyChanged(() => HasStop);
            }
        }

        private bool _garantedStop;
        public bool GarantedStop
        {
            get { return _garantedStop; }
            set
            {
                if (_garantedStop == value)
                    return;

                _garantedStop = value;
                RaisePropertyChanged(() => GarantedStop);
            }
        }

        private int _stopDistance;
        public int StopDistance
        {
            get { return _stopDistance; }
            set
            {
                if (_stopDistance == value)
                    return;

                _stopDistance = value;
                RaisePropertyChanged(() => StopDistance);
            }
        }

        private bool _hasLimit;
        public bool HasLimit
        {
            get { return _hasLimit; }
            set
            {
                if (_hasLimit == value)
                    return;

                _hasLimit = value;
                RaisePropertyChanged(() => HasLimit);
            }
        }

        private int _limitDistance;
        public int LimitDistance
        {
            get { return _limitDistance; }
            set
            {
                if (_limitDistance == value)
                    return;

                _limitDistance = value;
                RaisePropertyChanged(() => LimitDistance);
            }
        }

        private MarketPeriodSettingsViewModel _topLeftPeriod;
        public MarketPeriodSettingsViewModel TopLeftPeriod
        {
            get { return _topLeftPeriod; }
            set
            {
                if (_topLeftPeriod == value)
                    return;

                _topLeftPeriod = value;
                RaisePropertyChanged(() => TopLeftPeriod);
            }
        }

        private MarketPeriodSettingsViewModel _bottomLeftPeriod;
        public MarketPeriodSettingsViewModel BottomLeftPeriod
        {
            get { return _bottomLeftPeriod; }
            set
            {
                if (_bottomLeftPeriod == value)
                    return;

                _bottomLeftPeriod = value;
                RaisePropertyChanged(() => BottomLeftPeriod);
            }
        }

        private MarketPeriodSettingsViewModel _rightPeriod;
        public MarketPeriodSettingsViewModel RightPeriod
        {
            get { return _rightPeriod; }
            set
            {
                if (_rightPeriod == value)
                    return;

                _rightPeriod = value;
                RaisePropertyChanged(() => RightPeriod);
            }
        }

        public MarketSettingsViewModel(MarketSettings epicSettings)
        {
            _epicSettings = epicSettings;

            Name = _epicSettings.Name;

            Size = _epicSettings.Size;

            HasStop = _epicSettings.HasStop;
            GarantedStop = _epicSettings.GarantedStop;
            StopDistance = _epicSettings.StopDistance;

            HasLimit = _epicSettings.HasLimit;
            LimitDistance = _epicSettings.LimitDistance;

            TopLeftPeriod = new MarketPeriodSettingsViewModel(_epicSettings.TopLeftPeriod);
            BottomLeftPeriod = new MarketPeriodSettingsViewModel(_epicSettings.BottomLeftPeriod);
            RightPeriod = new MarketPeriodSettingsViewModel(_epicSettings.RightPeriod);
        }

        public MarketSettings Validate()
        {
            _epicSettings.Size = Size;

            _epicSettings.HasStop = HasStop;
            _epicSettings.GarantedStop = GarantedStop;
            _epicSettings.StopDistance = StopDistance;

            _epicSettings.HasLimit = HasLimit;
            _epicSettings.LimitDistance = LimitDistance;

            _epicSettings.TopLeftPeriod = TopLeftPeriod.Validate();
            _epicSettings.BottomLeftPeriod = BottomLeftPeriod.Validate();
            _epicSettings.RightPeriod = RightPeriod.Validate();

            return _epicSettings;
        }
    }
}
