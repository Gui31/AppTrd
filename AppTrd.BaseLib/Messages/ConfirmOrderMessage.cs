using GalaSoft.MvvmLight.Messaging;
using AppTrd.BaseLib.Model;

namespace AppTrd.BaseLib.Messages
{
    public class ConfirmOrderMessage : MessageBase
    {
        public ConfirmModel Confirm { get; private set; }

        public ConfirmOrderMessage(ConfirmModel confirm)
        {
            Confirm = confirm;
        }
    }
}