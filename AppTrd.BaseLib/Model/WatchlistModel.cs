using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace AppTrd.BaseLib.Model
{
    public class WatchlistModel : ViewModelBase
    {
        private string _watchlistName;
        public string WatchlistName
        {
            get { return _watchlistName; }
            set
            {
                if ((_watchlistName != value) && (value != null))
                {
                    _watchlistName = value;
                    RaisePropertyChanged("WatchlistName");
                }
            }
        }

        private string _watchlistId;
        public string WatchlistId
        {
            get { return _watchlistId; }
            set
            {
                if ((_watchlistId != value) && (value != null))
                {
                    _watchlistId = value;
                    RaisePropertyChanged("WatchlistId");
                }
            }
        }

        private bool _editable;
        public bool Editable
        {
            get { return _editable; }
            set
            {
                if (_editable != value)
                {
                    _editable = value;
                    RaisePropertyChanged("Editable");
                }
            }
        }

        private bool _deletable;
        public bool Deletable
        {
            get { return _deletable; }
            set
            {
                if (_deletable != value)
                {
                    _deletable = value;
                    RaisePropertyChanged("Deletable");
                }
            }
        }

    }
}
