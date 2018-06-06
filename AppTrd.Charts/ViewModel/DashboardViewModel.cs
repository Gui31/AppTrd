using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppTrd.BaseLib.Messages;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Service;
using AppTrd.BaseLib.ViewModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace AppTrd.Charts.ViewModel
{
    public class DashboardViewModel : BaseViewModel
    {
        private MainViewModel _mainViewModel;
        private ITradingService _tradingService;
        private bool _initialized = false;

        private AccountModel _account;
        public AccountModel Account
        {
            get { return _account; }
            set
            {
                if (_account == value)
                    return;

                _account = value;
                RaisePropertyChanged(() => Account);
            }
        }

        private ObservableCollection<PositionModel> _positions;
        public ObservableCollection<PositionModel> Positions
        {
            get { return _positions; }
            set
            {
                if (_positions == value)
                    return;

                _positions = value;
                RaisePropertyChanged(() => Positions);
            }
        }

        private bool _isTradingKeyboardActive;
        public bool IsTradingKeyboardActive
        {
            get { return _isTradingKeyboardActive; }
            set
            {
                if (_isTradingKeyboardActive == value)
                    return;

                _isTradingKeyboardActive = value;
                RaisePropertyChanged(() => IsTradingKeyboardActive);
            }
        }

        public RelayCommand OpenMarketCommand { get; }
        public RelayCommand OpenSettingsCommand { get; }
        public RelayCommand<PositionModel> OpenGraphCommand { get; }

        public DashboardViewModel(BaseMainViewModel mainViewModel, ITradingService tradingService)
        {
            _mainViewModel = mainViewModel as MainViewModel;
            _tradingService = tradingService;

            Title = "Dashboard";

            OpenMarketCommand = new RelayCommand(OpenMarket);
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            
            OpenGraphCommand = new RelayCommand<PositionModel>(OpenGraph);
        }

        public override void Init()
        {
            if (_initialized == false)
            {
                Account = _tradingService.CurrentAccount;

                Positions = new ObservableCollection<PositionModel>(_tradingService.Positions);

                Messenger.Default.Register<PositionAddedMessage>(this, PositionAddedMessageReceived);
                Messenger.Default.Register<PositionDeletedMessage>(this, PositionDeletedMessageReceived);

                _initialized = true;
            }
        }

        private void PositionAddedMessageReceived(PositionAddedMessage position)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Positions.Add(position.Position);
            });
        }

        private void PositionDeletedMessageReceived(PositionDeletedMessage position)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Positions.Remove(position.Position);
            });
        }

        private void OpenMarket()
        {
            _mainViewModel.SelectMarket();
        }

        private void OpenSettings()
        {
            _mainViewModel.DisplaySettings();
        }

        private void OpenGraph(PositionModel position)
        {
            if (position != null)
            {
                _mainViewModel.OpenMarket(position.Instrument.Epic);
            }
        }
    }
}
