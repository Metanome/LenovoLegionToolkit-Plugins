using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
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
                if (_targetPercent != value)
                {
                    _targetPercent = value;
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

        private FanType _type = FanType.Cpu;
        public FanType Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type != value)
                {
                    _type = value;
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
            return JsonConvert.SerializeObject(new { Type, CurveNodes = CurveNodes.ToList() }, Formatting.Indented);
        }

        public static CustomFanCurveEntry ImportFromJson(string json)
        {
            dynamic? data = JsonConvert.DeserializeObject(json);
            if (data == null)
            {
                throw new InvalidOperationException("Invalid JSON");
            }

            var entry = new CustomFanCurveEntry();
            if (data.Type != null)
            {
                entry.Type = data.Type;
            }

            if (data.CurveNodes != null)
            {
                entry.CurveNodes.Clear();
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
