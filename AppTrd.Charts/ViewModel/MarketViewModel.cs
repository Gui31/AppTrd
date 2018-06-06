using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using dto.endpoint.marketdetails.v2;
using GalaSoft.MvvmLight.Messaging;
using AppTrd.BaseLib.Common;
using AppTrd.BaseLib.Listener;
using AppTrd.BaseLib.ViewModel;
using AppTrd.BaseLib.Messages;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Receiver;
using AppTrd.BaseLib.Service;
using AppTrd.Charts.Setting;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;

namespace AppTrd.Charts.ViewModel
{
    public class MarketViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private readonly ITradingService _tradingService;
        private readonly ISettingsService _settingsService;
        private KeyboardSettings _keyboardSettings;
        private string _currency;
        private string _marketEpic;
        private InstrumentModel _instrument;

        private CandleReceiver _rightCandleReceiver;
        public CandleReceiver RightCandleReceiver
        {
            get { return _rightCandleReceiver; }
            set
            {
                if (_rightCandleReceiver == value)
                    return;

                _rightCandleReceiver = value;
                RaisePropertyChanged(() => RightCandleReceiver);
            }
        }

        private CandleReceiver _bottomLeftCandleReceiver;
        public CandleReceiver BottomLeftCandleReceiver
        {
            get { return _bottomLeftCandleReceiver; }
            set
            {
                if (_bottomLeftCandleReceiver == value)
                    return;

                _bottomLeftCandleReceiver = value;
                RaisePropertyChanged(() => BottomLeftCandleReceiver);
            }
        }

        private CandleReceiver _topLeftCandleReceiver;
        public CandleReceiver TopLeftCandleReceiver
        {
            get { return _topLeftCandleReceiver; }
            set
            {
                if (_topLeftCandleReceiver == value)
                    return;

                _topLeftCandleReceiver = value;
                RaisePropertyChanged(() => TopLeftCandleReceiver);
            }
        }

        private ObservableCollection<PositionModel> _positions = new ObservableCollection<PositionModel>();
        public ObservableCollection<PositionModel> Positions
        {
            get { return _positions; }
            private set
            {
                if (_positions == value)
                    return;

                _positions = value;
                RaisePropertyChanged(() => Positions);
            }
        }

        private int _doublePlacesFactor;
        public int DoublePlacesFactor
        {
            get { return _doublePlacesFactor; }
            set
            {
                if (_doublePlacesFactor == value)
                    return;

                _doublePlacesFactor = value;
                RaisePropertyChanged(() => DoublePlacesFactor);
            }
        }

        private AccountModel _account;
        public AccountModel Account
        {
            get { return _account; }
            set
            {
                if (_account == value)
                    return;

                _account = value;
                RaisePropertyChanged();
            }
        }

        private double _scalingFactor;
        public double ScalingFactor
        {
            get { return _scalingFactor; }
            set
            {
                if (_scalingFactor == value)
                    return;

                _scalingFactor = value;
                RaisePropertyChanged(() => ScalingFactor);
            }
        }

        private List<PivotPointModel> _pivotPoints;
        public List<PivotPointModel> PivotPoints
        {
            get { return _pivotPoints; }
            set
            {
                if (_pivotPoints == value)
                    return;

                _pivotPoints = value;
                RaisePropertyChanged(() => PivotPoints);
            }
        }

        private double _marketPnl;
        public double MarketPnl
        {
            get { return _marketPnl; }
            set
            {
                if (_marketPnl == value)
                    return;

                _marketPnl = value;
                RaisePropertyChanged(() => MarketPnl);
            }
        }

        private bool _hasPositions;
        public bool HasPositions
        {
            get { return _hasPositions; }
            set
            {
                if (_hasPositions == value)
                    return;

                _hasPositions = value;
                RaisePropertyChanged(() => HasPositions);
            }
        }

        public RelayCommand<Key> KeyPressCommand { get; }

        public MarketViewModel(MainViewModel mainViewModel, string epic)
        {
            _mainViewModel = mainViewModel;
            _tradingService = ServiceLocator.Current.GetInstance<ITradingService>();
            _settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();

            _marketEpic = epic;

            KeyPressCommand = new RelayCommand<Key>(KeyPress);
        }

        public override void Init()
        {
            _keyboardSettings = _settingsService.GetSettings<ChartsSettings>().Keyboard;

            Account = _tradingService.CurrentAccount;

            _instrument = _tradingService.GetInstrument(_marketEpic);

            _currency = _instrument.InstrumentData.currencies.First().code;
            DoublePlacesFactor = _instrument.SnapshotData.doublePlacesFactor;
            ScalingFactor = _instrument.SnapshotData.scalingFactor;

            Title = _instrument.InstrumentData.name;

            LoadMarketSettings();

            PivotPoints = _tradingService.GetPivotPoints(_marketEpic);

            _tradingService.SubscribeToChartCandle();

            foreach (var position in _tradingService.Positions.Where(p => p.Instrument.Epic == _marketEpic))
                AddPosition(position);

            Messenger.Default.Register<PositionAddedMessage>(this, _marketEpic, PositionAddedMessageReceived);
            Messenger.Default.Register<PositionDeletedMessage>(this, _marketEpic, PositionDeletedMessageReceived);
            Messenger.Default.Register<SettingsChangedMessage>(this, SettingsUpdated);
        }

        private void LoadMarketSettings()
        {
            var settings = _settingsService.GetSettings<ChartsSettings>();

            var marketSettings = settings?.Markets?.FirstOrDefault(m => m.Epic == _marketEpic);

            if (marketSettings == null)
            {
                marketSettings = MarketSettings.GetDefault(_marketEpic);

                if (settings.Markets == null)
                    settings.Markets = new List<MarketSettings>();

                settings.Markets.Add(marketSettings);

                _settingsService.SaveSettings<ChartsSettings>();
            }

            var tlp = marketSettings.TopLeftPeriod;
            if (tlp.Mode == PeriodMode.Time)
                TopLeftCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, (Periods)tlp.TimePeriod, tlp.AverageOpen);
            else
                TopLeftCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, tlp.TickCount, tlp.AverageOpen, tlp.MaxSeconds);

            var blp = marketSettings.BottomLeftPeriod;
            if (blp.Mode == PeriodMode.Time)
                BottomLeftCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, (Periods)blp.TimePeriod, blp.AverageOpen);
            else
                BottomLeftCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, blp.TickCount, blp.AverageOpen, blp.MaxSeconds);

            var rp = marketSettings.RightPeriod;
            if (rp.Mode == PeriodMode.Time)
                RightCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, (Periods)rp.TimePeriod, rp.AverageOpen);
            else
                RightCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, rp.TickCount, rp.AverageOpen, rp.MaxSeconds);
        }

        private void SettingsUpdated(SettingsChangedMessage message)
        {
            _keyboardSettings = _settingsService.GetSettings<ChartsSettings>().Keyboard;
        }

        private void KeyPress(Key key)
        {
        }

        private void PositionAddedMessageReceived(PositionAddedMessage position)
        {
            AddPosition(position.Position);
        }

        private void PositionDeletedMessageReceived(PositionDeletedMessage position)
        {
            RemovePosition(position.Position);
        }

        private void AddPosition(PositionModel position)
        {
            Positions.Add(position);

            position.PropertyChanged += PositionPropertyChanged;

            UpdatePnl();
        }

        private void RemovePosition(PositionModel position)
        {
            Positions.Remove(position);

            position.PropertyChanged += PositionPropertyChanged;

            UpdatePnl();
        }

        private void PositionPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PositionModel.Pnl))
                UpdatePnl();
        }

        private void UpdatePnl()
        {
            HasPositions = Positions.Any();
            MarketPnl = Positions.Sum(p => p.Pnl);
        }
    }
}
