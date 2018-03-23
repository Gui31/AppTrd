using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
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
    public class TicksDataReceiver : CandleReceiver, ITickReceiver
    {
        private DateTime _currentTime;
        private int _currentCount = 0;

        public int TicksCount { get; }
        public int AverageOpen { get; }
        public int MaxSeconds { get; }

        internal TicksDataReceiver(string epic, int ticksCount, int averageOpen, int maxSeconds)
            : base(epic)
        {
            TicksCount = ticksCount;
            AverageOpen = averageOpen;
            MaxSeconds = maxSeconds;

            Candles = new ObservableCollection<CandleData>();
        }

        public void AddTick(double bid, double ask)
        {
            var price = (bid + ask) / 2;
            var lastPrice = price;

            if (CurrentCandle == null || _currentCount >= TicksCount || (MaxSeconds > 0 && DateTime.Now.Subtract(_currentTime).TotalSeconds > MaxSeconds))
            {
                if (AverageOpen > 0 && Candles.Count >= AverageOpen)
                {
                    var up = CurrentCandle.Close > CurrentCandle.Open;

                    price = Candles.Reverse().Take(AverageOpen).Average(c => (c.Close + (up ? c.Low : c.High)) / 2);
                }

                CurrentCandle = new CandleData
                {
                    Time = DateTime.Now,
                    Open = price,
                    Close = lastPrice,
                    High = lastPrice,
                    Low = lastPrice,
                    LastPrice = lastPrice,
                    HasBidAsk = true,
                    Bid = bid,
                    Ask = ask
                };

                _currentCount = 1;
                _currentTime = DateTime.Now;
            }
            else
            {
                CurrentCandle.Close = price;
                CurrentCandle.High = Math.Max(Math.Max(CurrentCandle.High, lastPrice), price);
                CurrentCandle.Low = Math.Min(Math.Min(CurrentCandle.Low, lastPrice), price);
                CurrentCandle.LastPrice = lastPrice;
                CurrentCandle.Bid = bid;
                CurrentCandle.Ask = ask;

                _currentCount++;
            }
        }
    }
}
