using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Service;
using AppTrd.BaseLib.ViewModel;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;

namespace AppTrd.Options.ViewModel
{
    public class MainMenuViewModel : BaseViewModel
    {
        private ITradingService _tradingService;

        private List<string> _markets;
        public List<string> Markets
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

        public RelayCommand SelectOptionsCommand { get; }
        public RelayCommand<string> SelectMarketCommand { get; }

        public MainMenuViewModel(ITradingService tradingService)
        {
            _tradingService = tradingService;

            Title = "Main menu";

            SelectOptionsCommand = new RelayCommand(SelectOptions);
            SelectMarketCommand = new RelayCommand<string>(SelectMarket);
        }

        private void SelectOptions()
        {
            var mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();

            mainViewModel.SelectOptions();
        }

        private void SelectMarket(string market)
        {
            var mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();

            mainViewModel.SimulateOptions(market);
        }

        public override void Init()
        {
            var options = _tradingService.Positions.Where(p => p.Instrument.Epic.StartsWith("OP."));

            Markets = options.Select(o => o.Instrument.InstrumentData.marketId).Distinct().ToList();
        }
    }
}
