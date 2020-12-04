using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
namespace GTEventGenerator
{
    public class LocalSettings
    {
        public Dictionary<string, int> _settings;

        public LocalSettings()
        {
            _settings = new Dictionary<string, int>();
        }

        public void ReadFromFile(string file)
        {
            string[] lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                var spl = line.Split('|');
                if (spl.Length == 2)
                {
                    if (int.TryParse(spl[1], out int value))
                    {
                        if (!_settings.ContainsKey(spl[0]))
                            _settings.Add(spl[0], value);
                    }
                }
            }
        }

        public void CreateDefault()
        {
            SetSettingValue("Discord_Presence_Enabled", 0);
            SetSettingValue("Minify_XML", 1);
        }

        public void Save(string file)
        {
            using (var sw = new StreamWriter(file, false))
            {
                foreach (var setting in _settings)
                    sw.WriteLine($"{setting.Key}|{setting.Value}");
            }
        }

        public void DeleteSetting(string setting)
        {
            if (_settings.ContainsKey(setting))
                _settings.Remove(setting);
        }

        public void SetSettingValue(string setting, int value)
            => SetOrCreate(setting, value);

        public void SetSettingValue(string setting, bool value)
            => SetOrCreate(setting, value ? 1 : 0);

        public bool HasEnabledSetting(string setting)
            => _settings.TryGetValue(setting, out int value) && value == 1;
        
        private void SetOrCreate(string setting, int value)
        {
            if (!_settings.ContainsKey(setting))
                _settings.Add(setting, value);
            else
                _settings[setting] = value;
        }
    }
}
