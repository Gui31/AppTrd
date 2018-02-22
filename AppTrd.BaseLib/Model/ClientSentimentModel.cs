using GalaSoft.MvvmLight;

namespace AppTrd.BaseLib.Model
{
    public class ClientSentimentModel : ViewModelBase
    {
        private double? _clientShort;
        public double? ClientShort
        {
            get { return _clientShort; }
            set
            {
                if (_clientShort != value)
                {
                    _clientShort = value;
                    RaisePropertyChanged("ClientShort");
                }
            }
        }

        private double? _clientLong;
        public double? ClientLong
        {
            get { return _clientLong; }
            set
            {
                if (_clientLong != value)
                {
                    _clientLong = value;
                    RaisePropertyChanged("ClientLong");
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
    }
}