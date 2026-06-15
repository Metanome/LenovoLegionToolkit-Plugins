using System;
using System.IO;
using Newtonsoft.Json;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public sealed class PluginSettings
    {
        private const string FileName = "custom_fan_curve_settings.json";
        private readonly string _filePath;
        private CustomFanCurveSettings? _cached;

        public PluginSettings(string storagePath) => _filePath = Path.Combine(storagePath, FileName);

        public PluginSettings()
        {
            var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LenovoLegionToolkit", "Plugins", "CustomFanCurve");
            Directory.CreateDirectory(defaultPath);
            _filePath = Path.Combine(defaultPath, FileName);
        }

        public CustomFanCurveSettings Load()
        {
            if (_cached != null) return _cached;
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    _cached = JsonConvert.DeserializeObject<CustomFanCurveSettings>(json) ?? new CustomFanCurveSettings();
                    return _cached;
                }
            }
            catch (Exception ex) { Log.Instance.Trace($"CustomFanCurve load error: {ex.Message}"); }

            _cached = new CustomFanCurveSettings();
            Save(_cached);
            return _cached;
        }

        public void Save(CustomFanCurveSettings settings)
        {
            _cached = settings;
            try
            {
                var dir = Path.GetDirectoryName(_filePath);
                if (dir != null && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(settings, Formatting.Indented));
            }
            catch (Exception ex) { Log.Instance.Trace($"CustomFanCurve save error: {ex.Message}"); }
        }
    }
}
