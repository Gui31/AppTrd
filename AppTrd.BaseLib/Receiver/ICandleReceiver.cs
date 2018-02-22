using System;

namespace AppTrd.BaseLib.Receiver
{
    public interface ICandleReceiver
    {
        void CandleUpdate(DateTime time, double open, double close, double high, double low, bool end);
    }
}