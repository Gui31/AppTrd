using System;
using System.ComponentModel;
using AppTrd.BaseLib.ViewModel;

namespace AppTrd.BaseLib.Receiver
{
    public class CandleData : BaseViewModel
    {
        private DateTime _time;
        public DateTime Time
        {
            get { return _time; }
            set
            {
                if (_time == value)
                    return;

                _time = value;
                RaisePropertyChanged(() => Time);
            }
        }

        private double _open;
        public double Open
        {
            get { return _open; }
            set
            {
                if (_open == value)
                    return;

                _open = value;
                RaisePropertyChanged(() => Open);
            }
        }

        private double _close;
        public double Close
        {
            get { return _close; }
            set
            {
                if (_close == value)
                    return;

                _close = value;
                RaisePropertyChanged(() => Close);
            }
        }

        private double _low;
        public double Low
        {
            get { return _low; }
            set
            {
                if (_low == value)
                    return;

                _low = value;
                RaisePropertyChanged(() => Low);
            }
        }

        private double _high;
        public double High
        {
            get { return _high; }
            set
            {
                if (_high == value)
                    return;

                _high = value;
                RaisePropertyChanged(() => High);
            }
        }

        private double _lastPrice;
        public double LastPrice
        {
            get { return _lastPrice; }
            set
            {
                if (_lastPrice == value)
                    return;

                _lastPrice = value;
                RaisePropertyChanged(() => LastPrice);
            }
        }

        private bool _hasBidAsk;
        public bool HasBidAsk
        {
            get { return _hasBidAsk; }
            set
            {
                if (_hasBidAsk == value)
                    return;

                _hasBidAsk = value;
                RaisePropertyChanged(() => HasBidAsk);
            }
        }

        private double _bid;
        public double Bid
        {
            get { return _bid; }
            set
            {
                if (_bid == value)
                    return;

                _bid = value;
                RaisePropertyChanged(() => Bid);
            }
        }

        private double _ask;
        public double Ask
        {
            get { return _ask; }
            set
            {
                if (_ask == value)
                    return;

                _ask = value;
                RaisePropertyChanged(() => Ask);
            }
        }

        private bool _isEmpty;
        public bool IsEmpty
        {
            get { return _isEmpty; }
            set
            {
                if (_isEmpty == value)
                    return;

                _isEmpty = value;
                RaisePropertyChanged(() => IsEmpty);
            }
        }
    }
}