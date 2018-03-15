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
    public class CandleDataReceiver : CandleReceiver, ICandleReceiver
    {
        public Periods Period { get; }

        internal CandleDataReceiver(string epic, Periods period)
            : base(epic)
        {
            Period = period;

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
