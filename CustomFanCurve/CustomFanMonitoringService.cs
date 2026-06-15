using System;
using System.Collections.Generic;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public interface ICustomFanMonitoringService
    {
        event Action<FanType, FanMonitoringSnapshot>? MonitoringUpdated;
        IReadOnlyDictionary<FanType, FanMonitoringSnapshot> Current { get; }
        void Update(FanType type, float temp, int rpm, int targetRpm);
    }

    public class CustomFanMonitoringService : ICustomFanMonitoringService
    {
        private readonly Dictionary<FanType, FanMonitoringSnapshot> _data = new();

        public event Action<FanType, FanMonitoringSnapshot>? MonitoringUpdated;

        public IReadOnlyDictionary<FanType, FanMonitoringSnapshot> Current
        {
            get
            {
                return _data;
            }
        }

        public void Update(FanType type, float temp, int rpm, int targetRpm)
        {
            lock (_data)
            {
                _data[type] = new FanMonitoringSnapshot(temp, rpm, targetRpm);
            }

            MonitoringUpdated?.Invoke(type, _data[type]);
        }
    }

    public sealed record FanMonitoringSnapshot(float Temperature, int Rpm, int TargetRpm);
}
