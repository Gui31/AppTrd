using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dto.endpoint.marketdetails.v2;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using AppTrd.BaseLib.Common;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.ViewModel;
using AppTrd.BaseLib.Service;
using AppTrd.Options.ViewModel.Item;

namespace AppTrd.Options.ViewModel
{
    public class OptionsSimulatorViewModel : BaseViewModel
    {
        private ITradingService _tradingService;

        private List<OptionItem> _preparedOptions;

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

                if (value < 0)
                    throw new Exception();

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

                if (value <= MinPrice)
                    throw new Exception();

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

        public RelayCommand ResetCommand { get; }

        public OptionsSimulatorViewModel(ITradingService tradingService)
        {
            _tradingService = tradingService;

            Title = "Options simulation";

            GoBackCommand = new RelayCommand(GoBack);
            UpdateCommand = new RelayCommand(Update);
            ResetCommand = new RelayCommand(Reset);
        }

        private void GoBack()
        {
            var mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();

            mainViewModel.MainMenu();
        }

        public void Update()
        {
            LastUpdate = DateTime.Now;
        }

        private void Reset()
        {
            foreach (var option in Options)
            {
                var details = _tradingService.GetMarketDetails(option.Epic);

                option.Update(details);
            }

            CurrentPrice = Options.Average(o => o.CurrentPrice);

            Update();
        }

        public void Prepare(List<string> epics)
        {
            var options = new List<OptionItem>();

            foreach (var epic in epics)
            {
                var details = _tradingService.GetMarketDetails(epic);

                options.Add(new OptionItem(details));
            }

            _preparedOptions = options;
        }

        public void Prepare(string marketId)
        {
            var options = new List<OptionItem>();

            var positions = _tradingService.Positions.Where(p => p.Instrument.Epic.StartsWith("OP.") && p.Instrument.InstrumentData.marketId == marketId);

            foreach (var position in positions)
            {
                var details = _tradingService.GetMarketDetails(position.Instrument.Epic);

                var option = new OptionItem(details);

                option.Action = position.Direction == PositionModel.Directions.BUY ? OptionActions.Buy : OptionActions.Sell;
                option.CurrentPrime = position.OpenLevel;
                option.Quantity = position.DealSize * position.ContractSize;

                options.Add(option);
            }

            _preparedOptions = options;
        }

        public override void Init()
        {
            Options = _preparedOptions;

            CurrentPrice = Options.Average(o => o.CurrentPrice);

            EndDate = Options.Min(o => o.Expiry);
            MinPrice = Math.Round(Math.Min(CurrentPrice, Options.Min(p => p.Strike)) * 0.95);
            MaxPrice = Math.Round(Math.Max(CurrentPrice, Options.Max(o => o.Strike)) * 1.05);

            Update();
        }
    }
}
