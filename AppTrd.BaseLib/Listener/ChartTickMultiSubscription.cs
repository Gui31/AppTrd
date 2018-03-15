using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightstreamer.DotNet.Client;
using AppTrd.BaseLib.Common;
using AppTrd.BaseLib.Receiver;
using AppTrd.BaseLib.Service.Impl;
using IGWebApiClient;

namespace AppTrd.BaseLib.Listener
{
    public class ChartTickMultiSubscription : HandyTableListenerAdapter
    {
        private TradingService _tradingService;

        public ChartTickMultiSubscription(TradingService tradingService)
        {
            _tradingService = tradingService;
        }

        public override void OnUpdate(int itemPos, string itemName, IUpdateInfo update)
        {
            try
            {
                var receivers = _tradingService.CandleReceivers.OfType<TicksDataReceiver>().Where(r => itemName.Contains(r.Epic)).ToList();

                if (receivers.Count > 0)
                {
                    var bidStr = update.GetNewValue("BID");
                    var offerStr = update.GetNewValue("OFR");

                    if (bidStr == null || offerStr == null)
                        return;

                    var bid = Convert.ToDouble(bidStr);
                    var offer = Convert.ToDouble(offerStr);

                    receivers.ForEach(r => r.AddTick(bid, offer));
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
