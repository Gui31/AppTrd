using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace AppTrd.BaseLib.Model
{
    public class TradeSubscriptionModel : ViewModelBase
    {
        private string _tradeType;
        public string TradeType
        {
            get { return _tradeType; }
            set
            {
                if (_tradeType != value)
                {
                    _tradeType = value;
                    RaisePropertyChanged("TradeType");
                }
            }
        }

        private string _itemName;
        public string ItemName
        {
            get { return _itemName; }
            set
            {
                if (_itemName != value)
                {
                    _itemName = value;
                    RaisePropertyChanged("ItemName");
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
        private string _limitlevel;
        public string Limitlevel
        {
            get { return _limitlevel; }
            set
            {
                if (_limitlevel != value)
                {
                    _limitlevel = value;
                    RaisePropertyChanged("Limitlevel");
                }
            }
        }
        private string _dealId;
        public string DealId
        {
            get { return _dealId; }
            set
            {
                if (_dealId != value)
                {
                    _dealId = value;
                    RaisePropertyChanged("DealId");
                }
            }
        }
        private string _affectedDealId;
        public string AffectedDealId
        {
            get { return _affectedDealId; }
            set
            {
                if (_affectedDealId != value)
                {
                    _affectedDealId = value;
                    RaisePropertyChanged("AffectedDealId");
                }
            }
        }
        private string _stopLevel;
        public string StopLevel
        {
            get { return _stopLevel; }
            set
            {
                if (_stopLevel != value)
                {
                    _stopLevel = value;
                    RaisePropertyChanged("StopLevel");
                }
            }
        }

        private string _expiry;
        public string Expiry
        {
            get { return _expiry; }
            set
            {
                if (_expiry != value)
                {
                    _expiry = value;
                    RaisePropertyChanged("Expiry");
                }
            }
        }
        private string _size;
        public string Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    RaisePropertyChanged("Size");
                }
            }
        }
        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaisePropertyChanged("Status");
                }
            }
        }
        private string _epic;
        public string Epic
        {
            get { return _epic; }
            set
            {
                if (_epic != value)
                {
                    _epic = value;
                    RaisePropertyChanged("Epic");
                }
            }
        }
        private string _level;
        public string Level
        {
            get { return _level; }
            set
            {
                if (_level != value)
                {
                    _level = value;
                    RaisePropertyChanged("Level");
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
        private string _dealReference;
        public string DealReference
        {
            get { return _dealReference; }
            set
            {
                if (_dealReference != value)
                {
                    _dealReference = value;
                    RaisePropertyChanged("DealReference");
                }
            }
        }
        private string _dealStatus;
        public string DealStatus
        {
            get { return _dealStatus; }
            set
            {
                if (_dealStatus != value)
                {
                    _dealStatus = value;
                    RaisePropertyChanged("DealStatus");
                }
            }
        }

    }
}
