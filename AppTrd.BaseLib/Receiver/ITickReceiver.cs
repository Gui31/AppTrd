using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTrd.BaseLib.Receiver
{
    public interface ITickReceiver
    {
        void AddTick(double bid, double ask);
    }
}
