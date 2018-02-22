using GalaSoft.MvvmLight;

namespace AppTrd.BaseLib.Model
{
    public class OrderModel : ViewModelBase
    {
        private string _dealId;
        public string DealId
        {
            get { return _dealId; }
            set
            {
                if ((_dealId != value) && (value != null))
                {
                    _dealId = value;
                    RaisePropertyChanged("DealId");
                }
            }
        }

        private double? _orderSize;
        public double? OrderSize
        {
            get { return _orderSize; }
            set
            {
                if (_orderSize != value)
                {
                    _orderSize = value;
                    RaisePropertyChanged("OrderSize");
                }
            }
        }

        private double? _orderLevel;
        public double? OrderLevel
        {
            get { return _orderLevel; }
            set
            {
                if (_orderLevel != value)
                {
                    _orderLevel = value;
                    RaisePropertyChanged("OrderLevel");
                }
            }
        }

        private double? _stopDistance;
        public double? StopDistance
        {
            get { return _stopDistance; }
            set
            {
                if (_stopDistance != value)
                {
                    _stopDistance = value;
                    RaisePropertyChanged("StopDistance");
                }
            }
        }

        private double? _limitDistance;
        public double? LimitDistance
        {
            get { return _limitDistance; }
            set
            {
                if (_limitDistance != value)
                {
                    _limitDistance = value;
                    RaisePropertyChanged("LimitDistance");
                }
            }
        }

        private bool? _guaranteedStop;
        public bool? GuaranteedStop
        {
            get { return _guaranteedStop; }
            set
            {
                if (_guaranteedStop != value)
                {
                    _guaranteedStop = value;
                    RaisePropertyChanged("GuaranteedStop");
                }
            }
        }

        private string _orderType;
        public string OrderType
        {
            get { return _orderType; }
            set
            {
                if (_orderType != value)
                {
                    _orderType = value;
                    RaisePropertyChanged("OrderType");
                }
            }
        }

        private string _direction;
        public string Direction
        {
            get { return _direction; }
            set
            {
                if (_direction != value)
                {
                    _direction = value;
                    RaisePropertyChanged("Direction");
                }
            }
        }

        private string _creationDate;
        public string CreationDate
        {
            get { return _creationDate; }
            set
            {
                if ((_creationDate != value) && (value != null))
                {
                    _creationDate = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }

        private string _timeInForce;
        public string TimeInForce
        {
            get { return _timeInForce; }
            set
            {
                if ((_timeInForce != value) && (value != null))
                {
                    _timeInForce = value;
                    RaisePropertyChanged("TimeInForce");
                }
            }
        }

        private string _goodTillDate;
        public string GoodTillDate
        {
            get { return _goodTillDate; }
            set
            {
                if ((_goodTillDate != value) && (value != null))
                {
                    _goodTillDate = value;
                    RaisePropertyChanged("GoodTillDate");
                }
            }
        }

        private InstrumentModel _model;
        public InstrumentModel Model
        {
            get { return _model; }
            set
            {
                if ((_model != value) && (value != null))
                {
                    _model = value;
                    RaisePropertyChanged("Model");
                }
            }
        }
    }
}