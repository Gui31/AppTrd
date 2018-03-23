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

        public int AverageOpen { get; }

        internal CandleDataReceiver(string epic, Periods period, int averageOpen)
            : base(epic)
        {
            Period = period;
            AverageOpen = averageOpen;

            Candles = new ObservableCollection<CandleData>();

            LoadHistory();
        }

        public void CandleUpdate(DateTime time, double open, double close, double high, double low, bool end)
        {
            if (CurrentCandle == null || time != CurrentCandle.Time)
            {
                if (AverageOpen > 0 && Candles.Count >= AverageOpen)
                {
                    var up = CurrentCandle.Close > CurrentCandle.Open;

                    open = Candles.Reverse().Take(AverageOpen).Average(c => (c.Close + (up ? c.Low : c.High)) / 2);
                }

                CurrentCandle = new CandleData
                {
                    Time = time,
                    Open = open,
                    Close = close,
                    High = high,
                    Low = low,
                    LastPrice = close
                };
            }
            else
            {
                CurrentCandle.Close = close;
                CurrentCandle.High = high;
                CurrentCandle.Low = low;
                CurrentCandle.LastPrice = close;
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
                    var time = DateTime.Parse(price.snapshotTime);
                    var open = (double)(price.openPrice.ask.Value + price.openPrice.bid.Value) / 2;
                    var close = (double)(price.closePrice.ask.Value + price.closePrice.bid.Value) / 2;
                    var high = (double)(price.highPrice.ask.Value + price.highPrice.bid.Value) / 2;
                    var low = (double)(price.lowPrice.ask.Value + price.lowPrice.bid.Value) / 2;

                    CandleUpdate(time, open, close, high, low, true);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
