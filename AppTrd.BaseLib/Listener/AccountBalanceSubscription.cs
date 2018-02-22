using System;
using System.Linq;
using Lightstreamer.DotNet.Client;
using Microsoft.Practices.ServiceLocation;
using AppTrd.BaseLib.Service;
using AppTrd.BaseLib.Service.Impl;
using IGWebApiClient;

namespace AppTrd.BaseLib.Listener
{
    public class AccountBalanceSubscription : HandyTableListenerAdapter
    {
        private TradingService _tradingService;

        public AccountBalanceSubscription(TradingService tradingService)
        {
            _tradingService = tradingService;

            if (_tradingService == null)
                throw new ApplicationException("Wrong ITradingService implementation");
        }

        public override void OnUpdate(int itemPos, string itemName, IUpdateInfo update)
        {
            // TODO : vérifier l'account
            var accountUpdates = StreamingAccountDataUpdates(itemPos, itemName, update);

            if ((itemPos != 0) && (itemPos <= _tradingService.Accounts.Count))
            {
                var account = _tradingService.Accounts.FirstOrDefault(a => itemName.Contains(a.AccountId));

                if (account != null)
                {
                    account.AmountDue = accountUpdates.AmountDue;
                    account.AvailableCash = accountUpdates.AvailableCash;
                    account.Deposit = accountUpdates.Deposit;
                    account.ProfitLoss = accountUpdates.ProfitAndLoss;
                    account.UsedMargin = accountUpdates.UsedMargin;
                }
                else
                {

                }
            }
        }
    }
}