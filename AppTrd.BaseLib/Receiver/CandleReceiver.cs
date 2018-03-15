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
    public abstract class CandleReceiver : BaseViewModel
    {
        private CandleData _currentCandle;
        public CandleData CurrentCandle
        {
            get { return _currentCandle; }
            set
            {
                if (_currentCandle != null)
                    CandleClose?.Invoke(this, _currentCandle);

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

        public string Epic { get; }

        public event EventHandler<CandleData> CandleClose;

        internal CandleReceiver(string epic)
        {
            Epic = epic;

            Candles = new ObservableCollection<CandleData>();
        }
    }
}
