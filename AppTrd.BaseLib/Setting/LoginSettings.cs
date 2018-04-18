using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTrd.BaseLib.Setting
{
    public class LoginSettings : ISettings
    {
        public string SettingsName { get { return "Login"; } }

        public string Username { get; set; }
        public string ApiKey { get; set; }
        public bool UseDemo { get; set; }
        public bool SaveLogin { get; set; }
    }
}
