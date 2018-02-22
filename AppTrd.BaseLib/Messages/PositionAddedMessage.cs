using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using AppTrd.BaseLib.Model;

namespace AppTrd.BaseLib.Messages
{
    public class PositionAddedMessage : MessageBase
    {
        public PositionModel Position { get; private set; }

        public PositionAddedMessage(PositionModel position)
        {
            Position = position;
        }
    }
}
