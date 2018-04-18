using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Service;
using AppTrd.BaseLib.ViewModel;
using AppTrd.Charts.Setting;
using AppTrd.Charts.ViewModel.Settings;
using GalaSoft.MvvmLight.Command;

namespace AppTrd.Charts.ViewModel
{
    public class ChartsSettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private MainViewModel _mainViewModel;

        private KeyboardSettingsViewModel _keyboard;
        public KeyboardSettingsViewModel Keyboard
        {
            get { return _keyboard; }
            set
            {
                if (_keyboard == value)
                    return;

                _keyboard = value;
                RaisePropertyChanged(() => Keyboard);
            }
        }

        private List<MarketSettingsViewModel> _markets;
        public List<MarketSettingsViewModel> Markets
        {
            get { return _markets; }
            set
            {
                if (_markets == value)
                    return;

                _markets = value;
                RaisePropertyChanged(() => Markets);
            }
        }

        private MarketSettingsViewModel _selectedMarket;
        public MarketSettingsViewModel SelectedMarket
        {
            get { return _selectedMarket; }
            set
            {
                if (_selectedMarket == value)
                    return;

                _selectedMarket = value;
                RaisePropertyChanged(() => SelectedMarket);
                RaisePropertyChanged(() => MarketVisibility);
            }
        }

        public Visibility MarketVisibility
        {
            get { return SelectedMarket != null ? Visibility.Visible : Visibility.Collapsed; }
        }

        public RelayCommand CancelCommand { get; }
        public RelayCommand ValidateCommand { get; }

        public ChartsSettingsViewModel(BaseMainViewModel mainViewModel, ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _mainViewModel = mainViewModel as MainViewModel;

            Title = "Settings";

            CancelCommand = new RelayCommand(Cancel);
            ValidateCommand = new RelayCommand(Validate);
        }

        public override void Init()
        {
            var settings = _settingsService.GetSettings<ChartsSettings>();

            Keyboard = new KeyboardSettingsViewModel(settings.Keyboard);

            if (settings.Markets != null)
                Markets = settings.Markets.Select(e => new MarketSettingsViewModel(e)).ToList();
        }

        private void Cancel()
        {
            _mainViewModel.CloseSettings();
        }

        private void Validate()
        {
            var settings = _settingsService.GetSettings<ChartsSettings>();

            settings.Keyboard = Keyboard.Validate();

            if (Markets != null)
                settings.Markets = Markets.Select(m => m.Validate()).ToList();

            _settingsService.SaveSettings<ChartsSettings>();

            _mainViewModel.CloseSettings();
        }
    }
}
