using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Service;
using AppTrd.BaseLib.Setting;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;

namespace AppTrd.BaseLib.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly BaseMainViewModel _mainViewModel;
        private readonly ITradingService _tradingService;
        private readonly ISettingsService _settingsService;
        private string _username;
        private List<AccountModel> _accounts;
        private AccountModel _selectedAccount;
        private string _apiKey;
        private string _errorMessage;
        private bool _isLogged;
        private bool _useDemo = true;
        private bool _isBusy;

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username == value)
                    return;

                _username = value;
                RaisePropertyChanged();
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                if (_apiKey == value)
                    return;

                _apiKey = value;
                RaisePropertyChanged();
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public List<AccountModel> Accounts
        {
            get { return _accounts; }
            set
            {
                if (_accounts == value)
                    return;

                _accounts = value;
                RaisePropertyChanged();
            }
        }

        public AccountModel SelectedAccount
        {
            get { return _selectedAccount; }
            set
            {
                if (_selectedAccount == value)
                    return;

                _selectedAccount = value;
                RaisePropertyChanged();
                ConnectCommand.RaiseCanExecuteChanged();
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage == value)
                    return;

                _errorMessage = value;
                RaisePropertyChanged();
            }
        }

        public bool IsLogged
        {
            get { return _isLogged; }
            set
            {
                if (_isLogged == value)
                    return;

                _isLogged = value;
                RaisePropertyChanged();
            }
        }

        public bool UseDemo
        {
            get { return _useDemo; }
            set
            {
                if (_useDemo == value)
                    return;

                _useDemo = value;
                RaisePropertyChanged();
            }
        }

        private bool _saveLogin;
        public bool SaveLogin
        {
            get { return _saveLogin; }
            set
            {
                if (_saveLogin == value)
                    return;

                _saveLogin = value;
                RaisePropertyChanged(() => SaveLogin);
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy == value)
                    return;

                _isBusy = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<string> LoginCommand { get; private set; }
        public RelayCommand ConnectCommand { get; private set; }

        public LoginViewModel(BaseMainViewModel mainViewModel, ITradingService tradingService, ISettingsService settingsService)
        {
            _mainViewModel = mainViewModel;
            _tradingService = tradingService;
            _settingsService = settingsService;

            Title = "Login";

            LoadSettings();

            LoginCommand = new RelayCommand<string>(Login, p => !IsLogged || (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(p) && string.IsNullOrEmpty(ApiKey)));
            ConnectCommand = new RelayCommand(Connect, () => _tradingService.IsLogged && SelectedAccount != null);
        }

        private async void Login(string password)
        {
            IsBusy = true;
            ErrorMessage = null;

            var result = await Task.Run(() => _tradingService.Login(Username, password, ApiKey, UseDemo));

            if (result == false)
            {
                ErrorMessage = "Connexion impossible.";
                IsBusy = false;
                return;
            }

            SaveSettings(Username, ApiKey, UseDemo, SaveLogin);

            IsLogged = _tradingService.IsLogged;
            Accounts = _tradingService.Accounts;
            SelectedAccount = Accounts.FirstOrDefault(a => a.IsDefault);

            LoginCommand.RaiseCanExecuteChanged();
            ConnectCommand.RaiseCanExecuteChanged();

            IsBusy = false;
        }

        private async void Connect()
        {
            IsBusy = true;
            ErrorMessage = null;

            var result = await Task.Run(() => _tradingService.SelectAccount(SelectedAccount));

            if (result == false)
            {
                ErrorMessage = "Connexion impossible.";
                IsBusy = false;
                return;
            }

            IsLogged = false;

            Username = null;
            ApiKey = null;

            Accounts = null;
            SelectedAccount = null;

            _mainViewModel.LoginCompleted();
            IsBusy = false;
        }

        private void LoadSettings()
        {
            var settings = _settingsService.GetSettings<LoginSettings>();

            if (settings != null)
            {
                _username = settings.Username;
                _apiKey = settings.ApiKey;
                _useDemo = settings.UseDemo;
                _saveLogin = settings.SaveLogin;
            }
        }

        private void SaveSettings(string username, string apiKey, bool useDemo, bool saveLogin)
        {
            var settings = _settingsService.GetSettings<LoginSettings>();

            if (saveLogin)
            {
                settings = new LoginSettings
                {
                    Username = username,
                    ApiKey = apiKey,
                    UseDemo = useDemo,
                    SaveLogin = saveLogin
                };
            }
            else
            {
                settings = null;
            }

            _settingsService.SaveSettings<LoginSettings>(settings);
        }
    }
}
