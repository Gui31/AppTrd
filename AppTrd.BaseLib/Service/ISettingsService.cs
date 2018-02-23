using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTrd.BaseLib.Model;

namespace AppTrd.BaseLib.Service
{
    public interface ISettingsService
    {
        SettingsModel GetSettings();

        void SaveSettings();
    }
}
