using System;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers.Sensors;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    internal class SensorProvider
    {
        private readonly SensorsGroupController _controller;
        private bool _started;

        public event Action<HardwareSensorSnapshot>? SensorUpdated;
        public bool IsRunning
        {
            get
            {
                return _started;
            }
        }

        public HardwareSensorSnapshot? LatestSnapshot
        {
            get
            {
                return _controller.Snapshot;
            }
        }

        public SensorProvider(SensorsGroupController controller)
        {
            _controller = controller;
            _controller.SensorsUpdated += s => SensorUpdated?.Invoke(s);
        }

        public void Start(int intervalMs)
        {
            if (_started)
            {
                return;
            }

            _controller.Start(this, TimeSpan.FromMilliseconds(intervalMs));
            _started = true;
        }

        public void Stop()
        {
            if (!_started)
            {
                return;
            }

            _controller.Stop(this);
            _started = false;
        }
    }
}
