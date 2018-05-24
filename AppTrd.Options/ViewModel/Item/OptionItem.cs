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
        public string Epic { get; }

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

        private string _primeString;
        public string PrimeString
        {
            get { return _primeString; }
            set
            {
                if (_primeString == value)
                    return;

                _primeString = value;
                RaisePropertyChanged(() => PrimeString);
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

                if (_action == OptionActions.Buy)
                    CurrentPrime = Ask;

                if (_action == OptionActions.Sell)
                    CurrentPrime = Bid;
            }
        }

        private double _bid;
        public double Bid
        {
            get { return _bid; }
            set
            {
                if (_bid == value)
                    return;

                _bid = value;
                RaisePropertyChanged(() => Bid);
            }
        }

        private double _ask;
        public double Ask
        {
            get { return _ask; }
            set
            {
                if (_ask == value)
                    return;

                _ask = value;
                RaisePropertyChanged(() => Ask);
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
            Epic = marketDetails.instrument.epic;

            Quantity = 1;
            Name = $"{marketDetails.instrument.name} ({marketDetails.instrument.expiry})";

            var infos = marketDetails.instrument.name.Split(' ');

            var dir = infos[infos.Length - 1];
            var strike = infos[infos.Length - 2];

            Directions = dir.ToUpper() == "CALL" ? OptionDirections.Call : OptionDirections.Put;
            Strike = Convert.ToDouble(strike);

            Expiry = Convert.ToDateTime(marketDetails.instrument.expiryDetails.lastDealingDate, CultureInfo.GetCultureInfo("fr-FR"));

            Bid = Convert.ToDouble(marketDetails.snapshot.bid);
            Ask = Convert.ToDouble(marketDetails.snapshot.offer);

            Prime = (Bid + Ask) / 2;
            CurrentPrime = Prime;
            Spread = Ask - Bid;

            PrimeString = $"({Ask}/{Bid})";

            InterestRate = EuriborHelper.GetInterestRate(Expiry.Subtract(DateTime.Now));

            CurrentPrice = Convert.ToDouble(marketDetails.snapshot.netChange);
            var isCall = Directions == OptionDirections.Call;
            var time = Expiry.Subtract(DateTime.Now).TotalDays / 365;

            var correctedPrime = Math.Min(Bid + Spread * (Bid / 20), Prime);

            Volatility = BlackScholesHelper.ImpliedVolatility(isCall, CurrentPrice, Strike, time, 0, correctedPrime);

            InterestRate = Math.Round(InterestRate * 100, 2);
            Volatility = Math.Round(Volatility * 100, 2);
        }

        public void Update(MarketDetailsResponse marketDetails)
        {
            Bid = Convert.ToDouble(marketDetails.snapshot.bid);
            Ask = Convert.ToDouble(marketDetails.snapshot.offer);

            Prime = (Bid + Ask) / 2;
            Spread = Ask - Bid;

            InterestRate = EuriborHelper.GetInterestRate(Expiry.Subtract(DateTime.Now));

            CurrentPrice = Convert.ToDouble(marketDetails.snapshot.netChange);
            var isCall = Directions == OptionDirections.Call;
            var time = Expiry.Subtract(DateTime.Now).TotalDays / 365;

            var correctedPrime = Math.Min(Bid + Spread * (Bid / 20), Prime);

            Volatility = BlackScholesHelper.ImpliedVolatility(isCall, CurrentPrice, Strike, time, 0, correctedPrime);

            InterestRate = Math.Round(InterestRate * 100, 2);
            Volatility = Math.Round(Volatility * 100, 2);
        }
    }
}
