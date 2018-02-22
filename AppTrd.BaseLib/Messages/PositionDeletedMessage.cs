using GalaSoft.MvvmLight.Messaging;
using AppTrd.BaseLib.Model;

namespace AppTrd.BaseLib.Messages
{
    public class PositionDeletedMessage : MessageBase
    {
        public PositionModel Position { get; private set; }

        public PositionDeletedMessage(PositionModel position)
        {
            Position = position;
        }
    }
}