using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Windows;
using dto.endpoint.marketdetails.v2;
using dto.endpoint.positions.close.v1;
using dto.endpoint.positions.create.otc.v2;
using dto.endpoint.positions.create.sprintmarkets.v1;
using dto.endpoint.positions.edit.v2;
using dto.endpoint.prices.v2;
using GalaSoft.MvvmLight.Messaging;
using Lightstreamer.DotNet.Client;
using AppTrd.BaseLib.Common;
using AppTrd.BaseLib.Listener;
using AppTrd.BaseLib.Messages;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Receiver;
using dto.endpoint.auth.session.v2;
using IGWebApiClient;

namespace AppTrd.BaseLib.Service.Impl
{
    public partial class TradingService : ITradingService
    {
        private Dictionary<Periods, string> _periodsNames = new Dictionary<Periods, string>
        {
            {Periods.OneSecond, "SECOND"},
            {Periods.OneMinute, "MINUTE"},
            {Periods.TwoMinutes, "MINUTE_2"},
            {Periods.ThreeMinutes, "MINUTE_3"},
            {Periods.FiveMinutes, "MINUTE_5"},
            {Periods.TenMinutes, "MINUTE_10"},
            {Periods.FifteenMinutes, "MINUTE_15"},
            {Periods.ThirtYMinutes, "MINUTE_30"},
            {Periods.OneHour, "HOUR"},
            {Periods.TwoHours, "HOUR_2"},
            {Periods.ThreeHours, "HOUR_3"},
            {Periods.FourHours, "HOUR_4"},
            {Periods.OneDay, "DAY"},
            {Periods.OneWeek, "WEEK"},
            {Periods.OneMonth, "MONTH"},
        };

        private IgRestApiClient _igRestApiClient;
        private IGStreamingApiClient _igStreamApiClient;

        private SubscribedTableKey _accountBalanceStk;
        private SubscribedTableKey _tradeSubscriptionStk;
        private SubscribedTableKey _watchlistL1PricesSubscribedTableKey;
        private SubscribedTableKey _candleMultiSubKey;
        private SubscribedTableKey _ticksMultiSubKey;

        private readonly AccountBalanceSubscription _accountBalanceSub;
        private readonly TradeSubscription _tradeSub;
        private readonly L1PricesSubscription _l1PricesSub;
        private readonly ChartCandleMultiSubscription _candleMultiSub;
        private readonly ChartTickMultiSubscription _tickMultiSub;

        public bool IsLogged { get; private set; }
        public bool IsConnected { get; private set; }

        public List<AccountModel> Accounts { get; set; }
        public AccountModel CurrentAccount { get; set; }

        public List<InstrumentModel> Instruments { get; set; }
        public List<PositionModel> Positions { get; set; }
        public List<OrderModel> Orders { get; set; }
        public List<CandleReceiver> CandleReceivers { get; set; }

        public TradingService()
        {
            _accountBalanceSub = new AccountBalanceSubscription(this);
            _tradeSub = new TradeSubscription(this);
            _l1PricesSub = new L1PricesSubscription(this);
            _candleMultiSub = new ChartCandleMultiSubscription(this);
            _tickMultiSub = new ChartTickMultiSubscription(this);

            Instruments = new List<InstrumentModel>();
            Positions = new List<PositionModel>();
            Orders = new List<OrderModel>();
            CandleReceivers = new List<CandleReceiver>();

            Application.Current.Exit += Current_Exit;
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            this.Dispose();
        }

        public bool Login(string username, string password, string apiKey, bool useDemo)
        {
            if (IsLogged)
            {
                throw new Exception();
            }

            _igRestApiClient = new IgRestApiClient(useDemo);
            _igStreamApiClient = new IGStreamingApiClient();

            var ar = new AuthenticationRequest();
            ar.identifier = username;
            ar.password = password;

            try
            {
                var response = _igRestApiClient.SecureAuthenticate(ar, apiKey).RunSync();

                if (response && (response.Response != null) && (response.Response.accounts.Count > 0))
                {
                    if (Accounts == null)
                        Accounts = new List<AccountModel>();

                    Accounts.Clear();

                    foreach (var account in response.Response.accounts)
                    {
                        var model = new AccountModel();

                        model.ClientId = response.Response.clientId;
                        model.ProfitLoss = response.Response.accountInfo.profitLoss;
                        model.AvailableCash = response.Response.accountInfo.available;
                        model.Deposit = response.Response.accountInfo.deposit;
                        model.Balance = response.Response.accountInfo.balance;
                        model.LsEndpoint = response.Response.lightstreamerEndpoint;
                        model.AvailableCash = response.Response.accountInfo.available;
                        model.Balance = response.Response.accountInfo.balance;

                        model.AccountId = account.accountId;
                        model.AccountName = account.accountName;
                        model.AccountType = account.accountType;

                        model.IsDefault = account.accountId == response.Response.currentAccountId;

                        Accounts.Add(model);
                    }

                    IsLogged = true;
                }
            }
            catch (Exception ex)
            {
            }

            return IsLogged;
        }

        public bool SelectAccount(AccountModel account)
        {
            if (IsConnected)
            {
                throw new Exception();
            }

            CurrentAccount = account;

            ConversationContext context = _igRestApiClient.GetConversationContext();

            if ((context != null) && (account.LsEndpoint != null) && (context.apiKey != null) && (context.xSecurityToken != null) && (context.cst != null))
            {
                try
                {
                    var connectionEstablished = _igStreamApiClient.Connect(account.AccountId,
                                                                           context.cst,
                                                                           context.xSecurityToken, context.apiKey,
                                                                           account.LsEndpoint);
                    if (connectionEstablished)
                    {
                        LoadPositions();
                        LoadOrders();

                        IsConnected = true;

                        SubscribeToAccountDetails();
                        SubscribeToTrades();
                        SubscribeL1WatchlistPrices();
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return IsConnected;
        }

        public void Logout(bool stayConnected)
        {
            if (_igRestApiClient != null && stayConnected == false)
                _igRestApiClient.logout();

            if (_igStreamApiClient != null)
                _igStreamApiClient.OnClose();
        }

        private void SubscribeL1WatchlistPrices()
        {
            if (IsConnected == false)
                return;

            try
            {
                if (_igStreamApiClient != null)
                {
                    if (_watchlistL1PricesSubscribedTableKey != null)
                        _igStreamApiClient.UnsubscribeTableKey(_watchlistL1PricesSubscribedTableKey);

                    if (Instruments == null || Instruments.Count == 0)
                        return;

                    var epics = Instruments.Select(m => m.Epic).Where(e => e != "ZZ.Z.UNPRICED.DEAL.IP").ToList();

                    if (epics.Count == 0)
                        return;

                    _watchlistL1PricesSubscribedTableKey = _igStreamApiClient.SubscribeToMarketDetails(epics.ToArray(), _l1PricesSub);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void SubscribeToAccountDetails()
        {
            try
            {
                if (CurrentAccount != null)
                {
                    _accountBalanceStk = _igStreamApiClient.SubscribeToAccountDetails(CurrentAccount.AccountId, _accountBalanceSub);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void SubscribeToTrades()
        {
            try
            {
                if (CurrentAccount != null)
                {
                    _tradeSubscriptionStk = _igStreamApiClient.subscribeToTradeSubscription(CurrentAccount.AccountId, _tradeSub);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void Dispose()
        {
            if (_accountBalanceStk != null)
                _igStreamApiClient.UnsubscribeTableKey(_accountBalanceStk);

            if (_tradeSubscriptionStk != null)
                _igStreamApiClient.UnsubscribeTableKey(_tradeSubscriptionStk);

            if (_watchlistL1PricesSubscribedTableKey != null)
                _igStreamApiClient.UnsubscribeTableKey(_watchlistL1PricesSubscribedTableKey);

            if (_candleMultiSubKey != null)
                _igStreamApiClient.UnsubscribeTableKey(_candleMultiSubKey);

            Logout(true);
        }
    }
}
