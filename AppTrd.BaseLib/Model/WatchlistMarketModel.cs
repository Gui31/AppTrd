using GalaSoft.MvvmLight;

namespace AppTrd.BaseLib.Model
{
    public class WatchlistMarketModel : ViewModelBase
    {
        private string _updateTime;
        public string UpdateTime
        {
            get { return _updateTime; }
            set
            {
                if ((_updateTime != value) && (value != null))
                {
                    _updateTime = value;
                    RaisePropertyChanged("UpdateTime");
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