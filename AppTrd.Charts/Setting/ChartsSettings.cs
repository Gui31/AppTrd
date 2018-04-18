using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTrd.BaseLib.Setting;

namespace AppTrd.Charts.Setting
{
    public class ChartsSettings : ISettings
    {
        public string SettingsName => "Charts";

        public KeyboardSettings Keyboard { get; set; }

        public List<MarketSettings> Markets { get; set; }
    }
}
