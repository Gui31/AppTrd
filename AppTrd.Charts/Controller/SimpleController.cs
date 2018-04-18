using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTrd.BaseLib.Receiver;

namespace AppTrd.Charts.Controller
{
    public class SimpleController : IDisposable
    {
        private readonly CandleReceiver _receiver;
        private readonly int _periode;

        public SimpleController(CandleReceiver receiver, int periode)
        {
            if (periode < 1)
                throw new ArgumentException(nameof(periode));

            _receiver = receiver;
            _periode = periode;

            _receiver.CandleClose += ReceiverCandleClose;
        }

        private void ReceiverCandleClose(object sender, CandleData e)
        {
            if (_receiver.Candles.Count < 20)
                return;

            var average = _receiver.Candles.Skip(_receiver.Candles.Count - _periode).Average(c => c.Close);


        }

        public void Dispose()
        {
            _receiver.CandleClose -= ReceiverCandleClose;
        }
    }
}
