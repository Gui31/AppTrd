using AppTrd.BaseLib.Common;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;

namespace AppTrd.BaseLib.ViewModel
{
    public abstract class BaseMainViewModel : BaseViewModel
    {
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                if (_currentViewModel == value)
                    return;

                _currentViewModel = value;
                RaisePropertyChanged(() => CurrentViewModel);
            }
        }

        public override void Init()
        {
            CurrentViewModel = ServiceLocator.Current.GetInstance<LoginViewModel>();
        }

        public abstract void LoginCompleted();
    }
}