using System.Windows.Input;
using AppTrd.BaseLib.ViewModel;
using AppTrd.Charts.Setting;

namespace AppTrd.Charts.ViewModel.Settings
{
    public class KeyboardSettingsViewModel : BaseViewModel
    {
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

        public KeyboardSettingsViewModel(KeyboardSettings keyboardSettings)
        {
            BuyKey = null;
            SellKey = null;
            CloseAllKey = null;

            if (keyboardSettings == null)
                return;

            if (keyboardSettings.BuyKey != null)
                BuyKey = keyboardSettings.BuyKey.Key;

            if (keyboardSettings.SellKey != null)
                SellKey = keyboardSettings.SellKey.Key;

            if (keyboardSettings.CloseAllKey != null)
                CloseAllKey = keyboardSettings.CloseAllKey.Key;
        }

        public KeyboardSettings Validate()
        {
            var ks = new KeyboardSettings();

            if (BuyKey.HasValue)
                ks.BuyKey = new KeyValue { Key = BuyKey.Value };

            if (SellKey.HasValue)
                ks.SellKey = new KeyValue { Key = SellKey.Value };

            if (CloseAllKey.HasValue)
                ks.CloseAllKey = new KeyValue { Key = CloseAllKey.Value };

            return ks;
        }
    }
}
