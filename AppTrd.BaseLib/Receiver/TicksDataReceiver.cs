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
    public class TicksDataReceiver : CandleReceiver, ITickReceiver
    {
        private int _currentCount = 0;

        public int TicksCount { get; }

        internal TicksDataReceiver(string epic, int ticksCount)
            : base(epic)
        {
            TicksCount = ticksCount;

            Candles = new ObservableCollection<CandleData>();
        }

        public void AddTick(double bid, double ask)
        {
            var price = (bid + ask) / 2;

            if (CurrentCandle == null || _currentCount >= TicksCount)
            {
                CurrentCandle = new CandleData
                {
                    Time = DateTime.Now,
                    Open = price,
                    Close = price,
                    High = price,
                    Low = price
                };

                _currentCount = 1;
            }
            else
            {
                CurrentCandle.Close = price;
                CurrentCandle.High = Math.Max(CurrentCandle.High, price);
                CurrentCandle.Low = Math.Min(CurrentCandle.Low, price);

                _currentCount++;
            }
        }
    }
}
