using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Service;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace AppTrd.BaseLib.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly BaseMainViewModel _mainViewModel;
        private readonly ITradingService _tradingService;
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

        public LoginViewModel(BaseMainViewModel mainViewModel, ITradingService tradingService)
        {
            LoadRegistry();

            _mainViewModel = mainViewModel;
            _tradingService = tradingService;

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

            SaveRegistry(Username, ApiKey, UseDemo, SaveLogin);

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

        private const string LoginKey = "Software\\AppTrd\\Login";

        private void LoadRegistry()
        {
            var loginKey = Registry.CurrentUser.OpenSubKey(LoginKey);

            if (loginKey != null)
            {
                try
                {
                    _username = Convert.ToString(loginKey.GetValue("Username"));
                    _apiKey = Convert.ToString(loginKey.GetValue("ApiKey"));
                    _useDemo = Convert.ToBoolean(loginKey.GetValue("UseDemo"));
                    _saveLogin = Convert.ToBoolean(loginKey.GetValue("SaveLogin"));
                }
                catch (Exception)
                {
                    Registry.CurrentUser.DeleteSubKey(LoginKey);
                }
            }
        }

        private void SaveRegistry(string username, string apiKey, bool useDemo, bool saveLogin)
        {
            try
            {
                Registry.CurrentUser.DeleteSubKey(LoginKey);
            }
            catch (Exception)
            {
            }

            if (saveLogin == false)
                return;

            var loginKey = Registry.CurrentUser.OpenSubKey(LoginKey);

            if (loginKey == null)
                loginKey = Registry.CurrentUser.CreateSubKey(LoginKey);

            try
            {
                loginKey.SetValue("Username", username, RegistryValueKind.String);
                loginKey.SetValue("ApiKey", apiKey, RegistryValueKind.String);
                loginKey.SetValue("UseDemo", useDemo, RegistryValueKind.DWord);
                loginKey.SetValue("SaveLogin", saveLogin, RegistryValueKind.DWord);
            }
            catch (Exception)
            {
                Registry.CurrentUser.DeleteSubKey(LoginKey);
            }
        }
    }
}
