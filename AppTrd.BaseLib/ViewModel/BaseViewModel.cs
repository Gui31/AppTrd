using GalaSoft.MvvmLight;

namespace AppTrd.BaseLib.ViewModel
{
    public abstract class BaseViewModel : ViewModelBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value)
                    return;

                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public virtual void Init()
        {
        }
    }
}