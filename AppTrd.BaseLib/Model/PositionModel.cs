using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Microsoft.Win32.SafeHandles;

namespace AppTrd.BaseLib.Model
{
    public class PositionModel : ViewModelBase
    {
        public enum Directions
        {
            BUY,
            SELL,
        }

        private DateTime _createdDate;
        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set
            {
                if ((_createdDate != value))
                {
                    _createdDate = value;
                    RaisePropertyChanged("CreatedDate");
                }
            }
        }

        private DateTime _expirity;
        public DateTime Expirity
        {
            get { return _expirity; }
            set
            {
                if (_expirity == value)
                    return;

                _expirity = value;
                RaisePropertyChanged(() => Expirity);
            }
        }

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

        private double _dealSize;
        public double DealSize
        {
            get { return _dealSize; }
            set
            {
                if (_dealSize != value)
                {
                    _dealSize = value;
                    RaisePropertyChanged("DealSize");
                    RaisePropertyChanged("DealSizeWithDirection");
                }
            }
        }

        private Directions _direction;
        public Directions Direction
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

        private double _openLevel;
        public double OpenLevel
        {
            get { return _openLevel; }
            set
            {
                if (_openLevel != value)
                {
                    _openLevel = value;
                    RaisePropertyChanged("OpenLevel");
                    RaisePropertyChanged("OpenLevelText");

                    UpdatePnl();
                }
            }
        }

        private double? _stopLevel;
        public double? StopLevel
        {
            get { return _stopLevel; }
            set
            {
                if (_stopLevel != value)
                {
                    _stopLevel = value;
                    RaisePropertyChanged("StopLevel");
                    RaisePropertyChanged("StopLevelText");
                }
            }
        }

        private double? _limitLevel;
        public double? LimitLevel
        {
            get { return _limitLevel; }
            set
            {
                if (_limitLevel != value)
                {
                    _limitLevel = value;
                    RaisePropertyChanged("LimitLevel");
                    RaisePropertyChanged("LimitLevelText");
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

        private double _pnl;
        public double Pnl
        {
            get { return _pnl; }
            set
            {
                if (_pnl != value)
                {
                    _pnl = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _pNLText;
        public string PnlText
        {
            get { return _pNLText; }
            set
            {
                if (_pNLText == value)
                    return;

                _pNLText = value;
                RaisePropertyChanged(() => PnlText);
            }
        }

        private bool _isLocked;
        public bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _contractSize;
        public double ContractSize
        {
            get { return _contractSize; }
            set
            {
                if (_contractSize == value)
                    return;

                _contractSize = value;
                RaisePropertyChanged(() => ContractSize);
            }
        }

        private InstrumentModel _instrument;
        public InstrumentModel Instrument
        {
            get { return _instrument; }
            set
            {
                if ((_instrument != value) && (value != null))
                {
                    if (_instrument != null)
                    {
                        _instrument.PropertyChanged -= ModelPropertyChanged;
                    }

                    _instrument = value;
                    RaisePropertyChanged("Model");

                    if (_instrument != null)
                    {
                        _instrument.PropertyChanged += ModelPropertyChanged;

                        double contractSize;

                        if (double.TryParse(_instrument.InstrumentData.contractSize, out contractSize) == false)
                            contractSize = 1;

                        ContractSize = contractSize;

                        UpdatePnl();
                    }
                }
            }
        }

        private void ModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdatePnl();
        }

        public double DealSizeWithDirection
        {
            get { return Direction == Directions.SELL ? -_dealSize : _dealSize; }
        }

        public string doubleFormat { get; set; }

        public string OpenLevelText { get { return OpenLevel.ToString(doubleFormat); } }

        public string StopLevelText { get { return StopLevel.HasValue ? StopLevel.Value.ToString(doubleFormat) : null; } }

        public string LimitLevelText { get { return LimitLevel.HasValue ? LimitLevel.Value.ToString(doubleFormat) : null; } }

        private void UpdatePnl()
        {
            var model = Instrument;

            if (model != null)
            {
                if (Direction == Directions.BUY && model.Bid.HasValue)
                    Pnl = (model.Bid.Value - OpenLevel) * this.DealSize * ContractSize;

                if (Direction == Directions.SELL && model.Offer.HasValue)
                    Pnl = (OpenLevel - model.Offer.Value) * this.DealSize * ContractSize;
            }
        }
    }
}
