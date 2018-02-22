using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dto.endpoint.marketdetails.v2;
using GalaSoft.MvvmLight;

namespace AppTrd.BaseLib.Model
{
    public class InstrumentModel : ViewModelBase
    {
        private ClientSentimentModel _clientSentiment;
        public ClientSentimentModel ClientSentiment
        {
            get { return _clientSentiment; }
            set
            {
                if (_clientSentiment != value)
                {
                    _clientSentiment = value;
                    RaisePropertyChanged("ClientSentiment");
                }
            }
        }

        private string _marketStatus;
        public string MarketStatus
        {
            get { return _marketStatus; }
            set
            {
                if (_marketStatus != value)
                {
                    _marketStatus = value;
                    RaisePropertyChanged("MarketStatus");
                }
            }
        }

        private string _lsItemName;
        public string LsItemName
        {
            get { return _lsItemName; }
            set
            {
                if ((_lsItemName != value) && (value != null))
                {
                    _lsItemName = value;
                    RaisePropertyChanged("LsItemName");
                }
            }
        }

        private string _epic;
        public string Epic
        {
            get { return _epic; }
            set
            {
                if ((_epic != value) && (value != null))
                {
                    _epic = value;
                    RaisePropertyChanged("Epic");
                }
            }
        }

        private double? _bid;
        public double? Bid
        {
            get { return _bid; }
            set
            {
                if (_bid != value)
                {
                    _bid = value;
                    RaisePropertyChanged("Bid");
                }
            }
        }

        private double? _offer;
        public double? Offer
        {
            get { return _offer; }
            set
            {
                if (_offer != value)
                {
                    _offer = value;
                    RaisePropertyChanged("Offer");
                }
            }
        }

        private double? _open;
        public double? Open
        {
            get { return _open; }
            set
            {
                if (_open != value)
                {
                    _open = value;
                    RaisePropertyChanged("Open");
                }
            }
        }

        private string _instrumentName;
        public string InstrumentName
        {
            get
            {
                return _instrumentName;
            }
            set
            {
                if (_instrumentName != value)
                {
                    _instrumentName = value;
                    RaisePropertyChanged("InstrumentName");
                }
            }
        }

        private double? _netChange;
        public double? NetChange
        {
            get { return _netChange; }
            set
            {
                if (_netChange != value)
                {
                    _netChange = value;
                    RaisePropertyChanged("NetChange");
                }
            }
        }

        private double? _pctChange;
        public double? PctChange
        {
            get { return _pctChange; }
            set
            {
                if (_pctChange != value)
                {
                    _pctChange = value;
                    RaisePropertyChanged("PctChange");
                }
            }
        }

        private double? _high;
        public double? High
        {
            get { return _high; }
            set
            {
                if (_high != value)
                {
                    _high = value;
                    RaisePropertyChanged("High");
                }
            }
        }

        private double? _low;
        public double? Low
        {
            get { return _low; }
            set
            {
                if (_low != value)
                {
                    _low = value;
                    RaisePropertyChanged("Low");
                }
            }
        }

        private bool? _streamingPricesAvailable;
        public bool? StreamingPricesAvailable
        {
            get { return _streamingPricesAvailable; }
            set
            {
                if (_streamingPricesAvailable != value)
                {
                    _streamingPricesAvailable = value;
                    RaisePropertyChanged("StreamingPricesAvailable");
                }
            }
        }

        public DealingRulesData DealingRuleData { get; set; }
        public InstrumentData InstrumentData { get; set; }
        public MarketSnapshotData SnapshotData { get; set; }

        public InstrumentModel()
        {

        }
    }
}
