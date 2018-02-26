using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using AppTrd.BaseLib.Common;
using AppTrd.BaseLib.ViewModel;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Service;

namespace AppTrd.Charts.ViewModel
{
    public class MarketSelectorViewModel : BaseViewModel
    {
        private readonly ITradingService _tradingService;

        private readonly Stack<string> _browseHistory = new Stack<string>();

        public ObservableCollection<BrowseMarketModel> SelectedMarkets { get; } = new ObservableCollection<BrowseMarketModel>();

        private BrowseModel _browse;
        public BrowseModel Browse
        {
            get { return _browse; }
            set
            {
                if (_browse == value)
                    return;

                _browse = value;
                RaisePropertyChanged(() => Browse);
            }
        }

        public RelayCommand<string> BrowseNodeCommand { get; }
        public RelayCommand BrowseBackCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand<BrowseMarketModel> ValidateCommand { get; }

        public MarketSelectorViewModel(ITradingService tradingService)
        {
            _tradingService = tradingService;

            BrowseNodeCommand = new RelayCommand<string>(BrowseNode);
            BrowseBackCommand = new RelayCommand(BrowseBack);
            CancelCommand = new RelayCommand(Cancel);
            ValidateCommand = new RelayCommand<BrowseMarketModel>(Validate);

            Title = "Select market";
        }

        public override void Init()
        {
            SelectedMarkets.Clear();
            _browseHistory.Clear();

            Browse = BrowseRoot();
        }

        public void BrowseNode(string nodeId)
        {
            Browse = _tradingService.BrowseNode(nodeId);

            _browseHistory.Push(nodeId);
        }

        private void BrowseBack()
        {
            if (_browseHistory.Count == 0)
            {
                Cancel();
                return;
            }

            if (_browseHistory.Count > 0)
                _browseHistory.Pop();

            if (_browseHistory.Count > 0)
                Browse = _tradingService.BrowseNode(_browseHistory.Peek());
            else
                Browse = BrowseRoot();
        }

        private BrowseModel BrowseRoot()
        {
            var root = _tradingService.BrowseRoot();

            return root;
        }

        private void Cancel()
        {
            var mainViewModel = ServiceLocator.Current.GetInstance<BaseMainViewModel>() as MainViewModel;

            mainViewModel.DisplayDashboard();
        }

        private void Validate(BrowseMarketModel item)
        {
            var mainViewModel = ServiceLocator.Current.GetInstance<BaseMainViewModel>() as MainViewModel;

            mainViewModel.OpenMarket(item.Epic);
            mainViewModel.DisplayDashboard();
        }
    }
}
