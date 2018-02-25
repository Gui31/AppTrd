using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;

namespace AppTrd.BaseLib.Model
{
    public class SettingsModel
    {
        public LoginInformations LoginInformations { get; set; }

        public KeyboardSettings KeyboardSettings { get; set; }
    }

    public class KeyboardSettings
    {
        public KeyValue BuyKey { get; set; }

        public KeyValue SellKey { get; set; }

        public KeyValue CloseAllKey { get; set; }
    }

    public class KeyValue
    {
        public Key Key { get; set; }
    }

    public class LoginInformations
    {
        public string Username { get; set; }
        public string ApiKey { get; set; }
        public bool UseDemo { get; set; }
        public bool SaveLogin { get; set; }
    }
}
