using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.System.Management;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public interface ICustomFanHardware
    {
        bool IsSupported { get; }
        IReadOnlyList<int> AvailableFanIds { get; }
        Task InitializeAsync();
        int GetMaxRpm(int fanId);
        int GetMinRpm(int fanId);
        Task SetFanRpmAsync(int fanId, int rpm);
        Task<int> GetFanRpmAsync(int fanId);
    }

    internal class CustomFanHardware : ICustomFanHardware
    {
        private readonly Dictionary<int, int> _maxRpms = new();
        private readonly Dictionary<int, int> _minRpms = new();
        private readonly Dictionary<int, CapabilityID> _capabilityIds = new();
        private readonly List<int> _fanIds = new();

        public bool IsSupported { get; private set; }
        public IReadOnlyList<int> AvailableFanIds => _fanIds;

        public async Task InitializeAsync()
        {
            for (var fanId = 0; fanId <= 5; fanId++)
            {
                try
                {
                    var maxRpm = await WMI.LenovoFanTestData.GetFanMaxSpeedAsync(fanId).ConfigureAwait(false);
                    if (maxRpm > 0)
                    {
                        _fanIds.Add(fanId);
                        _maxRpms[fanId] = maxRpm;
                    }
                    else
                    {
                        continue;
                    }

                    var minRpm = await WMI.LenovoFanTestData.GetFanMinSpeedAsync(fanId).ConfigureAwait(false);
                    if (minRpm > 0)
                    {
                        _minRpms[fanId] = minRpm;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"FanHardware probe fail fanId={fanId}: {ex.Message}");
                }
            }

            foreach (var fanId in _fanIds)
            {
                _capabilityIds[fanId] = fanId switch
                {
                    2 => CapabilityID.GpuCurrentFanSpeed,
                    4 => CapabilityID.PchCurrentFanSpeed,
                    _ => CapabilityID.CpuCurrentFanSpeed,
                };
            }

            IsSupported = await CheckSupportAsync().ConfigureAwait(false);
        }

        private async Task<bool> CheckSupportAsync()
        {
            if (_maxRpms.Count == 0)
            {
                return false;
            }

            try
            {
                var rpm = await WMI.LenovoOtherMethod.GetFeatureValueAsync(CapabilityID.CpuCurrentFanSpeed).ConfigureAwait(false);
                return rpm >= 0;
            }
            catch
            {
                return false;
            }
        }

        public int GetMaxRpm(int fanId)
        {
            return _maxRpms.TryGetValue(fanId, out var r) ? r : 6400;
        }

        public int GetMinRpm(int fanId)
        {
            return _minRpms.TryGetValue(fanId, out var r) ? r : 1200;
        }

        public async Task SetFanRpmAsync(int fanId, int rpm)
        {
            if (!_capabilityIds.TryGetValue(fanId, out var cid))
            {
                return;
            }

            await WMI.LenovoOtherMethod.SetFeatureValueAsync(cid, Math.Max(0, rpm)).ConfigureAwait(false);
        }

        public async Task<int> GetFanRpmAsync(int fanId)
        {
            if (!_capabilityIds.TryGetValue(fanId, out var cid))
            {
                return 0;
            }

            try
            {
                return await WMI.LenovoOtherMethod.GetFeatureValueAsync(cid).ConfigureAwait(false);
            }
            catch
            {
                return 0;
            }
        }
    }
}
