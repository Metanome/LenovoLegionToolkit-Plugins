using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                int clampedValue = Math.Clamp(value, 0, 100);

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
            CurveNodes.Add(new CurveNode { Temperature = 0, TargetPercent = 0 });
            CurveNodes.Add(new CurveNode { Temperature = 10, TargetPercent = 10 });
            CurveNodes.Add(new CurveNode { Temperature = 20, TargetPercent = 20 });
            CurveNodes.Add(new CurveNode { Temperature = 30, TargetPercent = 30 });
            CurveNodes.Add(new CurveNode { Temperature = 40, TargetPercent = 40 });
            CurveNodes.Add(new CurveNode { Temperature = 50, TargetPercent = 50 });
            CurveNodes.Add(new CurveNode { Temperature = 60, TargetPercent = 60 });
            CurveNodes.Add(new CurveNode { Temperature = 70, TargetPercent = 70 });
            CurveNodes.Add(new CurveNode { Temperature = 80, TargetPercent = 80 });
            CurveNodes.Add(new CurveNode { Temperature = 90, TargetPercent = 90 });
            CurveNodes.Add(new CurveNode { Temperature = 100, TargetPercent = 100 });
            CurveNodes.CollectionChanged += (s, e) => OnPropertyChanged(nameof(CurveNodes));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
