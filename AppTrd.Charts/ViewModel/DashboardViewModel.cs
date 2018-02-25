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
        public RelayCommand<PositionModel> StopToZeroCommand { get; }
        public RelayCommand<PositionModel> LimitToZeroCommand { get; }
        public RelayCommand<PositionModel> SplitCommand { get; }
        public RelayCommand<PositionModel> CloseCommand { get; }
        public RelayCommand<PositionModel> LockCommand { get; }
        public RelayCommand<PositionModel> OpenGraphCommand { get; }

        public DashboardViewModel(BaseMainViewModel mainViewModel, ITradingService tradingService)
        {
            _mainViewModel = mainViewModel as MainViewModel;
            _tradingService = tradingService;

            OpenMarketCommand = new RelayCommand(OpenMarket);
            OpenSettingsCommand = new RelayCommand(OpenSettings);

            StopToZeroCommand = new RelayCommand<PositionModel>(StopToZero);
            LimitToZeroCommand = new RelayCommand<PositionModel>(LimitToZero);
            SplitCommand = new RelayCommand<PositionModel>(Split);
            CloseCommand = new RelayCommand<PositionModel>(Close);
            LockCommand = new RelayCommand<PositionModel>(Lock);
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

        private void StopToZero(PositionModel position)
        {
            if (position != null)
            {
                _tradingService.UpdateOrder(position, position.OpenLevel, position.LimitLevel);
            }
            else
            {
                var sumLevel = Positions.Sum(p => p.OpenLevel * p.DealSize);
                var sumSize = Positions.Sum(p => p.DealSize);

                var stopLevel = sumLevel / sumSize;

                foreach (var positionModel in Positions.Where(p => p.IsLocked == false))
                    _tradingService.UpdateOrder(positionModel, stopLevel, positionModel.LimitLevel);
            }
        }

        private void LimitToZero(PositionModel position)
        {
            if (position != null)
            {
                _tradingService.UpdateOrder(position, position.StopLevel, position.OpenLevel);
            }
            else
            {
                var sumLevel = Positions.Sum(p => p.OpenLevel * p.DealSize);
                var sumSize = Positions.Sum(p => p.DealSize);

                var limitLevel = sumLevel / sumSize;

                foreach (var positionModel in Positions.Where(p => p.IsLocked == false))
                    _tradingService.UpdateOrder(positionModel, positionModel.StopLevel, limitLevel);
            }
        }

        private void Split(PositionModel position)
        {
            double? buyLevel = null;
            double? sellLevel = null;

            //if (OrderType == 1)
            //{
            //    if (Model.Offer.HasValue)
            //        buyLevel = Model.Bid.Value;

            //    if (Model.Bid.HasValue)
            //        sellLevel = Model.Offer.Value;
            //}

            if (position != null)
            {
                _tradingService.CloseOrder(position, Math.Round(position.DealSize / 2, 2), position.Direction == PositionModel.Directions.BUY ? buyLevel : sellLevel);
            }
            else
            {
                foreach (var positionModel in Positions.Where(p => p.IsLocked == false))
                    _tradingService.CloseOrder(positionModel, Math.Round(positionModel.DealSize / 2, 2), positionModel.Direction == PositionModel.Directions.BUY ? buyLevel : sellLevel);
            }
        }

        private void Close(PositionModel position)
        {
            double? buyLevel = null;
            double? sellLevel = null;

            //if (OrderType == 1)
            //{
            //    if (Model.Offer.HasValue)
            //        buyLevel = Model.Offer.Value - 1;

            //    if (Model.Bid.HasValue)
            //        sellLevel = Model.Bid.Value + 1;
            //}

            if (position != null)
            {
                _tradingService.CloseOrder(position, position.DealSize, position.Direction == PositionModel.Directions.BUY ? buyLevel : sellLevel);
            }
            else
            {
                foreach (var positionModel in Positions.Where(p => p.IsLocked == false).ToList())
                    _tradingService.CloseOrder(positionModel, positionModel.DealSize, positionModel.Direction == PositionModel.Directions.BUY ? buyLevel : sellLevel);
            }
        }

        private void Lock(PositionModel position)
        {
            if (position != null)
            {
                position.IsLocked = !position.IsLocked;
            }
            else
            {
                foreach (var positionModel in Positions)
                    positionModel.IsLocked = false;
            }
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
