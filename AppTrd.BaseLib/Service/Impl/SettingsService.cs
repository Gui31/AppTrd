using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AppTrd.BaseLib.Messages;
using AppTrd.BaseLib.Model;
using GalaSoft.MvvmLight.Messaging;

namespace AppTrd.BaseLib.Service.Impl
{
    public class SettingsService : ISettingsService
    {
        private object _lock = new object();
        private XmlSerializer _xmlSerializer;
        private SettingsModel _settings;

        public SettingsModel GetSettings()
        {
            lock (_lock)
            {
                if (_settings == null)
                {
                    InitSerializer();

                    var filePath = GetSettingsFilePath();

                    if (File.Exists(filePath))
                    {
                        try
                        {
                            using (var stream = File.Open(filePath, FileMode.Open))
                            {
                                _settings = (SettingsModel)_xmlSerializer.Deserialize(stream);

                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                if (_settings == null)
                {
                    _settings = new SettingsModel();
                }

                return _settings;
            }
        }

        public void SaveSettings()
        {
            lock (_lock)
            {
                if (_settings != null)
                {
                    InitSerializer();

                    try
                    {
                        using (var stream = File.Open(GetSettingsFilePath(), FileMode.Create))
                        {
                            _xmlSerializer.Serialize(stream, _settings);

                            Messenger.Default.Send(new SettingsChangedMessage());
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void InitSerializer()
        {
            if (_xmlSerializer != null)
                return;

            _xmlSerializer = new XmlSerializer(typeof(SettingsModel));
        }

        private string GetSettingsFilePath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var directory = Path.Combine(appData, "AppTrd");

            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            return Path.Combine(directory, "Settings.xml");
        }
    }
}
