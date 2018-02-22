using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightstreamer.DotNet.Client;
using AppTrd.BaseLib.Common;
using AppTrd.BaseLib.Receiver;
using IGWebApiClient;

namespace AppTrd.BaseLib.Listener
{
    public class ChartTickSubscription : HandyTableListenerAdapter
    {
        private ITickReceiver _tickReceiver;

        public ChartTickSubscription(ITickReceiver tickReceiver)
        {
            _tickReceiver = tickReceiver;
        }

        public override void OnUpdate(int itemPos, string itemName, IUpdateInfo update)
        {
            try
            {
                if (_tickReceiver != null)
                {
                    var bidStr = update.GetNewValue("BID");
                    var offerStr = update.GetNewValue("OFR");

                    if (bidStr == null || offerStr == null)
                        return;

                    var bid = Convert.ToDouble(bidStr);
                    var offer = Convert.ToDouble(offerStr);

                    _tickReceiver.AddTick(bid, offer);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
