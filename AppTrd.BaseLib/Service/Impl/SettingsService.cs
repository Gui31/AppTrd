using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AppTrd.BaseLib.Messages;
using AppTrd.BaseLib.Model;
using AppTrd.BaseLib.Setting;
using GalaSoft.MvvmLight.Messaging;

namespace AppTrd.BaseLib.Service.Impl
{
    public class SettingsService : ISettingsService
    {
        private object _lock = new object();
        private readonly Dictionary<Type, XmlSerializer> _xmlSerializers = new Dictionary<Type, XmlSerializer>();
        private readonly List<ISettings> _settings = new List<ISettings>();

        public T GetSettings<T>() where T : ISettings, new()
        {
            lock (_lock)
            {
                var settings = _settings.OfType<T>().FirstOrDefault();

                if (settings == null)
                {
                    settings = new T();

                    var serializer = GetSerializer<T>();

                    var filePath = GetSettingsFilePath(settings.SettingsName);

                    if (File.Exists(filePath))
                    {
                        try
                        {
                            using (var stream = File.Open(filePath, FileMode.Open))
                            {
                                settings = (T)serializer.Deserialize(stream);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                    _settings.Add(settings);
                }

                return settings;
            }
        }

        public void SaveSettings<T>() where T : ISettings, new()
        {
            SaveSettings<T>(GetSettings<T>());
        }

        public void SaveSettings<T>(T settings) where T : ISettings, new()
        {
            lock (_lock)
            {
                var serializer = GetSerializer<T>();

                if (settings == null)
                    settings = new T();

                var filePath = GetSettingsFilePath(settings.SettingsName);

                var prev = _settings.OfType<T>().FirstOrDefault();

                if (prev != null)
                {
                    _settings.Remove(prev);
                    _settings.Add(settings);
                }

                try
                {
                    using (var stream = File.Open(filePath, FileMode.Create))
                    {
                        serializer.Serialize(stream, settings);

                        Messenger.Default.Send(new SettingsChangedMessage());
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private XmlSerializer GetSerializer<T>()
        {
            XmlSerializer serializer;

            if (_xmlSerializers.ContainsKey(typeof(T)) == false)
            {
                serializer = new XmlSerializer(typeof(T));

                _xmlSerializers.Add(typeof(T), serializer);
            }
            else
                serializer = _xmlSerializers[typeof(T)];

            return serializer;
        }

        private string GetSettingsFilePath(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException();

            var invalidChars = Path.GetInvalidFileNameChars();

            if (name.Any(c => invalidChars.Contains(c)))
                throw new ArgumentException();

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var directory = Path.Combine(appData, "AppTrd");

            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            return Path.Combine(directory, $"Settings_{name}.xml");
        }
    }
}
