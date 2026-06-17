using System;
using System.Collections.Generic;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public interface ICustomFanMonitoringService
    {
        event Action<int, FanMonitoringSnapshot>? MonitoringUpdated;
        IReadOnlyDictionary<int, FanMonitoringSnapshot> Current { get; }
        void Update(int fanId, float temp, int rpm, int targetRpm);
    }

    public class CustomFanMonitoringService : ICustomFanMonitoringService
    {
        private readonly Dictionary<int, FanMonitoringSnapshot> _data = new();

        public event Action<int, FanMonitoringSnapshot>? MonitoringUpdated;

        public IReadOnlyDictionary<int, FanMonitoringSnapshot> Current
        {
            get
            {
                return _data;
            }
        }

        public void Update(int fanId, float temp, int rpm, int targetRpm)
        {
            FanMonitoringSnapshot snapshot;
            lock (_data)
            {
                snapshot = new FanMonitoringSnapshot(temp, rpm, targetRpm);
                _data[fanId] = snapshot;
            }

            MonitoringUpdated?.Invoke(fanId, snapshot);
        }
    }

    public sealed record FanMonitoringSnapshot(float Temperature, int Rpm, int TargetRpm);
}
