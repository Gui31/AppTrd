using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dto.endpoint.marketdetails.v2;
using GalaSoft.MvvmLight;
using AppTrd.Options.Helper;

namespace AppTrd.Options.ViewModel.Item
{
    public enum OptionDirections
    {
        Call,
        Put
    }

    public enum OptionActions
    {
        None,
        Buy,
        Sell
    }

    public class OptionItem : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        private double _strike;
        public double Strike
        {
            get { return _strike; }
            set
            {
                if (_strike == value)
                    return;

                _strike = value;
                RaisePropertyChanged(() => Strike);
            }
        }

        private double _prime;
        public double Prime
        {
            get { return _prime; }
            set
            {
                if (_prime == value)
                    return;

                _prime = value;
                RaisePropertyChanged(() => Prime);
            }
        }

        private double _currentPrime;
        public double CurrentPrime
        {
            get { return _currentPrime; }
            set
            {
                if (_currentPrime == value)
                    return;

                _currentPrime = value;
                RaisePropertyChanged(() => CurrentPrime);
            }
        }

        private double _spread;
        public double Spread
        {
            get { return _spread; }
            set
            {
                if (_spread == value)
                    return;

                _spread = value;
                RaisePropertyChanged(() => Spread);
            }
        }

        private OptionDirections _directions;
        public OptionDirections Directions
        {
            get { return _directions; }
            set
            {
                if (_directions == value)
                    return;

                _directions = value;
                RaisePropertyChanged(() => Directions);
            }
        }

        private double _volatility;
        public double Volatility
        {
            get { return _volatility; }
            set
            {
                if (_volatility == value)
                    return;

                _volatility = value;
                RaisePropertyChanged(() => Volatility);
            }
        }

        private double _interestRate;
        public double InterestRate
        {
            get { return _interestRate; }
            set
            {
                if (_interestRate == value)
                    return;

                _interestRate = value;
                RaisePropertyChanged(() => InterestRate);
            }
        }

        private OptionActions _action;
        public OptionActions Action
        {
            get { return _action; }
            set
            {
                if (_action == value)
                    return;

                _action = value;
                RaisePropertyChanged(() => Action);
            }
        }

        private double _quantity;
        public double Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity == value)
                    return;

                _quantity = value;
                RaisePropertyChanged(() => Quantity);
            }
        }

        private DateTime _expiry;
        public DateTime Expiry
        {
            get { return _expiry; }
            set
            {
                if (_expiry == value)
                    return;

                _expiry = value;
                RaisePropertyChanged(() => Expiry);
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

        public OptionItem(MarketDetailsResponse marketDetails)
        {
            Quantity = 1;
            Name = $"{marketDetails.instrument.name} ({marketDetails.instrument.expiry})";

            var infos = marketDetails.instrument.name.Split(' ');

            var dir = infos[infos.Length - 1];
            var strike = infos[infos.Length - 2];

            Directions = dir.ToUpper() == "CALL" ? OptionDirections.Call : OptionDirections.Put;
            Strike = Convert.ToDouble(strike);

            Expiry = Convert.ToDateTime(marketDetails.instrument.expiryDetails.lastDealingDate, CultureInfo.GetCultureInfo("fr-FR"));

            var bid = Convert.ToDouble(marketDetails.snapshot.bid);
            var ask = Convert.ToDouble(marketDetails.snapshot.offer);

            Prime = (bid + ask) / 2;
            CurrentPrime = Prime;
            Spread = ask - bid;

            InterestRate = EuriborHelper.GetInterestRate(Expiry.Subtract(DateTime.Now));

            CurrentPrice = Convert.ToDouble(marketDetails.snapshot.netChange);
            var isCall = Directions == OptionDirections.Call;
            var time = Expiry.Subtract(DateTime.Now).TotalDays / 365;

            Volatility = BlackScholesHelper.ImpliedVolatility(isCall, CurrentPrice, Strike, time, 0, Prime);

            InterestRate = Math.Round(InterestRate * 100, 2);
            Volatility = Math.Round(Volatility * 100, 2);
        }
    }
}
