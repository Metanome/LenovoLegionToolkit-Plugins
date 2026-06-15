using System;
using System.Collections.Generic;
using System.Linq;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public class CustomFanCurveConfigManager
    {
        private readonly PluginSettings _store;
        private CustomFanCurveSettings _settings;
        private readonly object _lock = new();

        public CustomFanCurveSettings Settings => _settings;
        public event Action? SettingsChanged;

        public CustomFanCurveConfigManager(PluginSettings store)
        {
            _store = store;
            _settings = store.Load();
            EnsureDefaultEntriesExist();
        }

        private void EnsureDefaultEntriesExist()
        {
            lock (_lock)
            {
                var changed = false;
                foreach (var type in new[] { FanType.Cpu, FanType.Gpu, FanType.System })
                {
                    if (!_settings.Entries.Any(e => e.Type == type))
                    {
                        _settings.Entries.Add(new CustomFanCurveEntry { Type = type });
                        changed = true;
                    }
                }
                if (changed) { _store.Save(_settings); SettingsChanged?.Invoke(); }
            }
        }

        public void UpdateSetting<T>(string propertyName, T value)
        {
            lock (_lock)
            {
                var prop = typeof(CustomFanCurveSettings).GetProperty(propertyName);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(_settings, value);
                    _store.Save(_settings);
                    SettingsChanged?.Invoke();
                }
            }
        }

        public CustomFanCurveEntry? GetEntry(FanType type)
        {
            lock (_lock) { return _settings.Entries.FirstOrDefault(e => e.Type == type); }
        }

        public void SaveEntry(CustomFanCurveEntry entry)
        {
            lock (_lock)
            {
                var existing = _settings.Entries.FirstOrDefault(e => e.Type == entry.Type);
                if (existing != null) _settings.Entries.Remove(existing);
                _settings.Entries.Add(entry);
                _store.Save(_settings);
                SettingsChanged?.Invoke();
            }
        }

        public List<CustomFanCurveEntry> GetAllEntries()
        {
            lock (_lock) { return _settings.Entries.ToList(); }
        }
    }
}
