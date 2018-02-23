using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTrd.BaseLib.Model
{
    public class SettingsModel
    {
        public LoginInformations LoginInformations { get; set; }
    }

    public class LoginInformations
    {
        public string Username { get; set; }
        public string ApiKey { get; set; }
        public bool UseDemo { get; set; }
        public bool SaveLogin { get; set; }
    }
}
