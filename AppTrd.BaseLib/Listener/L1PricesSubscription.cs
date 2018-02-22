using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightstreamer.DotNet.Client;
using Microsoft.Practices.ServiceLocation;
using AppTrd.BaseLib.Service;
using AppTrd.BaseLib.Service.Impl;
using IGWebApiClient;

namespace AppTrd.BaseLib.Listener
{
    public class L1PricesSubscription : HandyTableListenerAdapter
    {
        private TradingService _tradingService;

        public L1PricesSubscription(TradingService tradingService)
        {
            _tradingService = tradingService;

            if (_tradingService == null)
                throw new ApplicationException("Wrong ITradingService implementation");
        }

        public override void OnUpdate(int itemPos, string itemName, IUpdateInfo update)
        {
            try
            {
                var wlmUpdate = L1LsPriceUpdateData(itemPos, itemName, update);

                var epic = itemName.Replace("L1:", "");

                var model = _tradingService.Instruments.FirstOrDefault(m => m.Epic == epic);

                if (model != null)
                {
                    model.Epic = epic;
                    model.Bid = wlmUpdate.Bid;
                    model.Offer = wlmUpdate.Offer;
                    model.NetChange = wlmUpdate.Change;
                    model.PctChange = wlmUpdate.ChangePct;
                    model.Low = wlmUpdate.Low;
                    model.High = wlmUpdate.High;
                    model.Open = wlmUpdate.MidOpen;
                    //_marketViewModel.UpdateTime = wlmUpdate.UpdateTime;
                    model.MarketStatus = wlmUpdate.MarketState;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
