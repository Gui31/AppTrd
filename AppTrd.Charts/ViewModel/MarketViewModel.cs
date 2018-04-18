﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
        private MarketDetailsResponse _marketDetails;
        private KeyboardSettings _keyboardSettings;
        private string _currency;
        private string _marketEpic;

        private CandleReceiver _oneMinuteCandleReceiver;
        public CandleReceiver OneMinuteCandleReceiver
        {
            get { return _oneMinuteCandleReceiver; }
            set
            {
                if (_oneMinuteCandleReceiver == value)
                    return;

                _oneMinuteCandleReceiver = value;
                RaisePropertyChanged(() => OneMinuteCandleReceiver);
            }
        }

        private CandleReceiver _fiveMinutesCandleReceiver;
        public CandleReceiver FiveMinutesCandleReceiver
        {
            get { return _fiveMinutesCandleReceiver; }
            set
            {
                if (_fiveMinutesCandleReceiver == value)
                    return;

                _fiveMinutesCandleReceiver = value;
                RaisePropertyChanged(() => FiveMinutesCandleReceiver);
            }
        }

        private CandleReceiver _oneHourCandleReceiver;
        public CandleReceiver OneHourCandleReceiver
        {
            get { return _oneHourCandleReceiver; }
            set
            {
                if (_oneHourCandleReceiver == value)
                    return;

                _oneHourCandleReceiver = value;
                RaisePropertyChanged(() => OneHourCandleReceiver);
            }
        }

        private ObservableCollection<PositionModel> _positions = new ObservableCollection<PositionModel>();
        public ObservableCollection<PositionModel> Positions
        {
            get { return _positions; }
            set
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

        private bool _isTradingKeyboardActive;
        public bool IsTradingKeyboardActive
        {
            get { return _isTradingKeyboardActive; }
            set
            {
                if (_isTradingKeyboardActive == value)
                    return;

                _isTradingKeyboardActive = value;
                RaisePropertyChanged(() => IsTradingKeyboardActive);
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

            _marketDetails = _tradingService.GetMarketDetails(_marketEpic);
            _currency = _marketDetails.instrument.currencies.First().code;
            DoublePlacesFactor = _marketDetails.snapshot.doublePlacesFactor;
            ScalingFactor = _marketDetails.snapshot.scalingFactor;

            Title = _marketDetails.instrument.name;

            //if (_marketDetails.dealingRules.minDealSize.value != null)
            //    Size = (double)_marketDetails.dealingRules.minDealSize.value.Value;
            //else
            //    Size = 1;

            //OneMinuteCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, Periods.OneMinute);
            //FiveMinutesCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, Periods.FiveMinutes);
            //OneHourCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, Periods.OneHour);

            //OneMinuteCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, Periods.OneMinute, 0);
            //FiveMinutesCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, Periods.OneMinute, 3);
            //OneHourCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, Periods.OneMinute, 7);

            //OneMinuteCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, 21, 3, 5);
            //FiveMinutesCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, Periods.OneMinute, 3);
            //OneHourCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, Periods.FiveMinutes, 0);

            LoadMarketSettings();

            _tradingService.SubscribeToChartCandle();

            foreach (var position in _tradingService.Positions.Where(p => p.Instrument.Epic == _marketEpic))
                Positions.Add(position);

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

            Size = marketSettings.Size;

            var tlp = marketSettings.TopLeftPeriod;
            if (tlp.Mode == PeriodMode.Time)
                OneHourCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, (Periods)tlp.TimePeriod, tlp.AverageOpen);
            else
                OneHourCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, tlp.TickCount, tlp.AverageOpen, tlp.MaxSeconds);

            var blp = marketSettings.BottomLeftPeriod;
            if (blp.Mode == PeriodMode.Time)
                FiveMinutesCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, (Periods)blp.TimePeriod, blp.AverageOpen);
            else
                FiveMinutesCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, blp.TickCount, blp.AverageOpen, blp.MaxSeconds);

            var rp = marketSettings.RightPeriod;
            if (rp.Mode == PeriodMode.Time)
                OneMinuteCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, (Periods)rp.TimePeriod, rp.AverageOpen);
            else
                OneMinuteCandleReceiver = _tradingService.GetCandleReceiver(_marketEpic, rp.TickCount, rp.AverageOpen, rp.MaxSeconds);
        }

        private void SettingsUpdated(SettingsChangedMessage message)
        {
            _keyboardSettings = _settingsService.GetSettings<ChartsSettings>().Keyboard;
        }

        private void KeyPress(Key key)
        {
            if (_keyboardSettings == null)
                return;

            if (_keyboardSettings.CloseAllKey != null && key == _keyboardSettings.CloseAllKey.Key)
                CloseAllPosition();

            if (IsTradingKeyboardActive == false)
                return;

            if (_keyboardSettings.BuyKey != null && key == _keyboardSettings.BuyKey.Key)
                _tradingService.CreateOrder("BUY", _marketEpic, _currency, Size, null, null, null, false);

            if (_keyboardSettings.SellKey != null && key == _keyboardSettings.SellKey.Key)
                _tradingService.CreateOrder("SELL", _marketEpic, _currency, Size, null, null, null, false);
        }

        public void CloseAllPosition()
        {
            foreach (var positionModel in Positions.ToList())
            {
                _tradingService.CloseOrder(positionModel, positionModel.DealSize, null);
            }

            Positions.Clear();
        }

        private void PositionAddedMessageReceived(PositionAddedMessage position)
        {
            Positions.Add(position.Position);
        }

        private void PositionDeletedMessageReceived(PositionDeletedMessage position)
        {
            Positions.Remove(position.Position);
        }
    }
}