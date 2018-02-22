using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dto.endpoint.marketdetails.v2;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using AppTrd.BaseLib.Common;
using AppTrd.BaseLib.ViewModel;
using AppTrd.BaseLib.Service;
using AppTrd.Options.ViewModel.Item;

namespace AppTrd.Options.ViewModel
{
    public class OptionsSimulatorViewModel : BaseViewModel
    {
        private ITradingService _tradingService;

        private List<OptionItem> _options;
        public List<OptionItem> Options
        {
            get { return _options; }
            set
            {
                if (_options == value)
                    return;

                _options = value;
                RaisePropertyChanged(() => Options);
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate == value)
                    return;

                _endDate = value;
                RaisePropertyChanged(() => EndDate);
            }
        }

        private double _minPrice;
        public double MinPrice
        {
            get { return _minPrice; }
            set
            {
                if (_minPrice == value)
                    return;

                _minPrice = value;
                RaisePropertyChanged(() => MinPrice);
            }
        }

        private double _maxPrice;
        public double MaxPrice
        {
            get { return _maxPrice; }
            set
            {
                if (_maxPrice == value)
                    return;

                _maxPrice = value;
                RaisePropertyChanged(() => MaxPrice);
            }
        }

        private double _currentPrice;
        public double CurrentPrice
        {
            get { return _currentPrice; }
            set
            {
                if (_currentPrice == value)
                    return;

                _currentPrice = value;
                RaisePropertyChanged(() => CurrentPrice);
            }
        }

        private DateTime _lastUpdate;
        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set
            {
                if (_lastUpdate == value)
                    return;

                _lastUpdate = value;
                RaisePropertyChanged(() => LastUpdate);
            }
        }

        public RelayCommand GoBackCommand { get; private set; }
        public RelayCommand UpdateCommand { get; private set; }

        public OptionsSimulatorViewModel(ITradingService tradingService)
        {
            _tradingService = tradingService;

            GoBackCommand = new RelayCommand(GoBack);
            UpdateCommand = new RelayCommand(Update);
        }

        private void GoBack()
        {
            var mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();

            mainViewModel.SelectOptions();
        }

        public void Update()
        {
            LastUpdate = DateTime.Now;
        }

        public override void Init()
        {
            var options = new List<OptionItem>();
            var selector = ServiceLocator.Current.GetInstance<OptionsSelectorViewModel>();

            foreach (var selectedMarket in selector.SelectedMarkets)
            {
                var details = _tradingService.GetMarketDetails(selectedMarket.Epic);

                options.Add(new OptionItem(details));
            }

            Options = options;

            CurrentPrice = Options.Average(o => o.CurrentPrice);

            EndDate = Options.Min(o => o.Expiry);
            MinPrice = Math.Min(CurrentPrice, Options.Min(p => p.Strike)) * 0.95;
            MaxPrice = Math.Max(CurrentPrice, Options.Max(o => o.Strike)) * 1.05;

            Update();
        }
    }
}
