using System;
using System.Diagnostics;
using Lightstreamer.DotNet.Client;
using AppTrd.BaseLib.Common;
using AppTrd.BaseLib.Receiver;
using IGWebApiClient;

namespace AppTrd.BaseLib.Listener
{
    public class ChartCandleSubscription : HandyTableListenerAdapter
    {
        private ICandleReceiver _candleReceiver;
        private readonly bool _closeOnly;

        public ChartCandleSubscription(ICandleReceiver candleReceiver, bool closeOnly = false)
        {
            _candleReceiver = candleReceiver;
            _closeOnly = closeOnly;
        }

        public DateTime FromUnixTime(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }

        private readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void OnUpdate(int itemPos, string itemName, IUpdateInfo update)
        {
            try
            {
                if (_candleReceiver != null)
                {
                    var bidStr = update.GetNewValue("BID_CLOSE");
                    var offerStr = update.GetNewValue("BID_CLOSE");

                    var end = update.GetNewValue("CONS_END");

                    var timeStr = update.GetNewValue("UTM");
                    DateTime time = DateTime.Now;

                    if (timeStr != null)
                    {
                        time = FromUnixTime(Convert.ToInt64(timeStr) / 1000).ToLocalTime();
                    }

                    if (string.IsNullOrEmpty(bidStr) || string.IsNullOrEmpty(offerStr))
                        return;

                    var close = (Convert.ToDouble(bidStr) + Convert.ToDouble(offerStr)) / 2;

                    var open = (Convert.ToDouble(update.GetNewValue("BID_OPEN")) +
                                Convert.ToDouble(update.GetNewValue("BID_OPEN"))) / 2;
                    var low = (Convert.ToDouble(update.GetNewValue("BID_LOW")) +
                               Convert.ToDouble(update.GetNewValue("BID_LOW"))) / 2;
                    var high = (Convert.ToDouble(update.GetNewValue("BID_HIGH")) +
                                Convert.ToDouble(update.GetNewValue("BID_HIGH"))) / 2;

                    _candleReceiver.CandleUpdate(time, open, close, high, low, end == "1");
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}