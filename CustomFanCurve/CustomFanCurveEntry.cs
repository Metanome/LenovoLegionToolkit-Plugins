using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using LenovoLegionToolkit.Lib;

using System.ComponentModel.DataAnnotations;
using LenovoLegionToolkit.Plugin.CustomFanCurve.Resources;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public enum SensorSource
    {
        [Display(Name = "SensorSourceDefault", ResourceType = typeof(Resource))]
        Default,
        [Display(Name = "SensorSourceMaxCpuGpu", ResourceType = typeof(Resource))]
        MaxCpuGpu,
        [Display(Name = "SensorSourceAverageCpuGpu", ResourceType = typeof(Resource))]
        AverageCpuGpu
    }

    public class CurveNode : INotifyPropertyChanged
    {
        private float _temperature;
        public float Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                if (Math.Abs(_temperature - value) > 0.01f)
                {
                    _temperature = value;
                    OnPropertyChanged();
                    
                    TargetPercent = _targetPercent;
                }
            }
        }

        private int _targetPercent;
        public int TargetPercent
        {
            get
            {
                return _targetPercent;
            }
            set
            {
                int safeMin = CustomFanCurveCalculator.GetSafeMinPercent(_temperature);
                int clampedValue = Math.Clamp(value, safeMin, 100);

                // BUG-14: Only fire PropertyChanged when the backing field actually changes.
                // The old else-if fired even when clampedValue == _targetPercent (e.g. slider below safe-min floor),
                // causing a debounced save on every drag event without any real data change.
                if (_targetPercent != clampedValue)
                {
                    _targetPercent = clampedValue;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CustomFanCurveEntry : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private int _fanId;
        public int FanId
        {
            get => _fanId;
            set
            {
                if (_fanId != value)
                {
                    _fanId = value;
                    OnPropertyChanged();
                }
            }
        }

        private SensorSource _sensorSource = SensorSource.Default;
        public SensorSource SensorSource
        {
            get => _sensorSource;
            set
            {
                if (_sensorSource != value)
                {
                    _sensorSource = value;
                    OnPropertyChanged();
                }
            }
        }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public ObservableCollection<CurveNode> CurveNodes { get; set; } = new();

        public CustomFanCurveEntry()
        {
            CurveNodes.Add(new CurveNode { Temperature = 40, TargetPercent = 0 });
            CurveNodes.Add(new CurveNode { Temperature = 50, TargetPercent = 30 });
            CurveNodes.Add(new CurveNode { Temperature = 60, TargetPercent = 50 });
            CurveNodes.Add(new CurveNode { Temperature = 70, TargetPercent = 70 });
            CurveNodes.Add(new CurveNode { Temperature = 80, TargetPercent = 85 });
            CurveNodes.Add(new CurveNode { Temperature = 90, TargetPercent = 100 });
            CurveNodes.CollectionChanged += (s, e) => OnPropertyChanged(nameof(CurveNodes));
        }

        public string ExportToJson()
        {
            return JsonConvert.SerializeObject(new { FanId, SensorSource, CurveNodes = CurveNodes.ToList() }, Formatting.Indented);
        }

        public static CustomFanCurveEntry ImportFromJson(string json)
        {
            dynamic? data = JsonConvert.DeserializeObject(json);
            if (data == null)
            {
                throw new InvalidOperationException("Invalid JSON");
            }

            var entry = new CustomFanCurveEntry();
            if (data.FanId != null)
            {
                entry.FanId = (int)data.FanId;
            }

            if (data.SensorSource != null)
            {
                entry.SensorSource = (SensorSource)Enum.Parse(typeof(SensorSource), (string)data.SensorSource, true);
            }

            // BUG-13: Always clear existing nodes before import, even if JSON has no nodes,
            // so default 6 nodes from the constructor don't survive a partial/empty import.
            entry.CurveNodes.Clear();
            if (data.CurveNodes != null)
            {
                foreach (var node in data.CurveNodes)
                {
                    entry.CurveNodes.Add(new CurveNode { Temperature = node.Temperature, TargetPercent = node.TargetPercent });
                }
            }

            return entry;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
