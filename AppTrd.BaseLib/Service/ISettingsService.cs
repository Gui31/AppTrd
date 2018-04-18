using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Setting;

namespace AppTrd.BaseLib.Service
{
    public interface ISettingsService
    {
        T GetSettings<T>() where T : ISettings, new();

        void SaveSettings<T>() where T : ISettings, new();

        void SaveSettings<T>(T settings) where T : ISettings, new();
    }
}
