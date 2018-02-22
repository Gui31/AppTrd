using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTrd.BaseLib.Listener;
using AppTrd.BaseLib.Messages;
using AppTrd.BaseLib.Service;
using AppTrd.BaseLib.Service.Impl;
using AppTrd.BaseLib.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using Lightstreamer.DotNet.Client;
using Microsoft.Practices.ServiceLocation;

namespace AppTrd.BaseLib.Receiver
{
    public class CandleReceiver : BaseViewModel, ICandleReceiver
    {
        private ChartCandleSubscription _chartCandleSubscription;
        private SubscribedTableKey _subscribedTableKey;

        private CandleData _currentCandle;
        public CandleData CurrentCandle
        {
            get { return _currentCandle; }
            set
            {
                if (_currentCandle == value)
                    return;

                _currentCandle = value;
                RaisePropertyChanged(() => CurrentCandle);

                if (value != null)
                    Candles.Add(value);
            }
        }

        private ObservableCollection<CandleData> _candles;
        public ObservableCollection<CandleData> Candles
        {
            get { return _candles; }
            set
            {
                if (_candles == value)
                    return;

                _candles = value;
                RaisePropertyChanged(() => Candles);
            }
        }

        public Periods Period { get; }

        public string Epic { get; }

        public CandleReceiver(string epic, Periods period)
        {
            Period = period;
            Epic = epic;

            Candles = new ObservableCollection<CandleData>();

            LoadHistory();
        }

        public void CandleUpdate(DateTime time, double open, double close, double high, double low, bool end)
        {
            if (CurrentCandle == null || time != CurrentCandle.Time)
            {
                CurrentCandle = new CandleData
                {
                    Time = time,
                    Open = open,
                    Close = close,
                    High = high,
                    Low = low
                };
            }
            else
            {
                CurrentCandle.Open = open;
                CurrentCandle.Close = close;
                CurrentCandle.High = high;
                CurrentCandle.Low = low;
            }
        }

        private void LoadHistory()
        {
            var tradingService = ServiceLocator.Current.GetInstance<ITradingService>();

            var prices = tradingService.GetPriceList(Epic, Period, 100);

            foreach (var price in prices.prices)
            {
                try
                {
                    var candle = new CandleData
                    {
                        Time = DateTime.Parse(price.snapshotTime),
                        Open = (double)(price.openPrice.ask.Value + price.openPrice.bid.Value) / 2,
                        Close = (double)(price.closePrice.ask.Value + price.closePrice.bid.Value) / 2,
                        High = (double)(price.highPrice.ask.Value + price.highPrice.bid.Value) / 2,
                        Low = (double)(price.lowPrice.ask.Value + price.lowPrice.bid.Value) / 2,
                    };

                    CurrentCandle = candle;
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
