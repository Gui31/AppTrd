using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Service;
using AppTrd.BaseLib.ViewModel;
using GalaSoft.MvvmLight.Command;

namespace AppTrd.Charts.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private MainViewModel _mainViewModel;

        private Key? _buyKey;
        public Key? BuyKey
        {
            get { return _buyKey; }
            set
            {
                if (_buyKey == value)
                    return;

                _buyKey = value;
                RaisePropertyChanged(() => BuyKey);
            }
        }

        private Key? _sellKey;
        public Key? SellKey
        {
            get { return _sellKey; }
            set
            {
                if (_sellKey == value)
                    return;

                _sellKey = value;
                RaisePropertyChanged(() => SellKey);
            }
        }

        private Key? _closeAllKey;
        public Key? CloseAllKey
        {
            get { return _closeAllKey; }
            set
            {
                if (_closeAllKey == value)
                    return;

                _closeAllKey = value;
                RaisePropertyChanged(() => CloseAllKey);
            }
        }

        public RelayCommand CancelCommand { get; }
        public RelayCommand ValidateCommand { get; }

        public SettingsViewModel(BaseMainViewModel mainViewModel, ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _mainViewModel = mainViewModel as MainViewModel;

            CancelCommand = new RelayCommand(Cancel);
            ValidateCommand = new RelayCommand(Validate);
        }

        public override void Init()
        {
            BuyKey = null;
            SellKey = null;
            CloseAllKey = null;

            var ks = _settingsService.GetSettings().KeyboardSettings;

            if (ks == null)
                return;

            if (ks.BuyKey != null)
                BuyKey = ks.BuyKey.Key;

            if (ks.SellKey != null)
                SellKey = ks.SellKey.Key;

            if (ks.CloseAllKey != null)
                CloseAllKey = ks.CloseAllKey.Key;
        }

        private void Cancel()
        {
            _mainViewModel.DisplayDashboard();
        }

        private void Validate()
        {
            var ks = new KeyboardSettings();
          
            if (BuyKey.HasValue)
                ks.BuyKey = new KeyValue {Key = BuyKey.Value};

            if (SellKey.HasValue)
                ks.SellKey = new KeyValue { Key = SellKey.Value };

            if (CloseAllKey.HasValue)
                ks.CloseAllKey = new KeyValue { Key = CloseAllKey.Value };

            var settings = _settingsService.GetSettings();

            settings.KeyboardSettings = ks;

            _settingsService.SaveSettings();

            _mainViewModel.DisplayDashboard();
        }
    }
}
