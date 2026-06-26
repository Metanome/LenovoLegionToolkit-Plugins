using System;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public static class FuzzyLogicFanController
    {
        private const int OutputSilent = 15;
        private const int OutputMaintain = -1;
        private const int OutputSlightRamp = 40;
        private const int OutputAggressive = 70;
        private const int OutputMax = 100;

        public static int CalculateSmartRpm(HardwareSensorSnapshot snapshot, float evalTemp, int maxRpm, int currentTargetRpm, int fanIndex)
        {
            if (evalTemp >= 90) return maxRpm;

            float temp = evalTemp;
            float power = snapshot.CpuPower;

            if (fanIndex == 1)
            {
                power = snapshot.GpuPower > 0 ? snapshot.GpuPower : snapshot.CpuPower;
            }
            else if (fanIndex > 1)
            {
                power = Math.Max(snapshot.CpuPower, snapshot.GpuPower);
            }

            float targetPercent = 0f;

            float loadFactor;
            if (power < 0)
            {
                float usage = fanIndex == 1 ? snapshot.GpuUsage : (fanIndex == 0 ? snapshot.CpuUsage : Math.Max(snapshot.CpuUsage, snapshot.GpuUsage));
                loadFactor = Math.Clamp((usage - 20f) / (70f - 20f), 0f, 1f);
            }
            else
            {
                loadFactor = Math.Clamp((power - 20f) / (65f - 20f), 0f, 1f);
            }

            float thermalPercent = Math.Clamp((temp - 50f) / (85f - 50f) * (OutputMax - OutputSlightRamp), 0f, OutputMax - OutputSlightRamp);

            float powerPercent = loadFactor * OutputSlightRamp;

            targetPercent = thermalPercent + powerPercent;

            if (temp >= 55 && targetPercent < OutputSilent)
            {
                targetPercent = OutputSilent;
            }

            if (temp < 45 && targetPercent < OutputSilent)
            {
                targetPercent = 0;
            }

            return (int)(maxRpm * (Math.Clamp(targetPercent, 0f, OutputMax) / OutputMax));
        }

        public static (string ThermalState, string PowerLoad, string Decision) GetGlobalTelemetryStrings(HardwareSensorSnapshot snapshot)
        {
            float maxTemp = Math.Max(snapshot.CpuTemp, snapshot.GpuTemp);
            float maxPower = Math.Max(snapshot.CpuPower, snapshot.GpuPower);

            string thermalState = maxTemp >= 90 ? "Critical" : maxTemp >= 75 ? "Hot" : maxTemp >= 55 ? "Warm" : "Cold";
            string powerLoad;
            string loadString;

            if (maxPower < 0)
            {
                float maxUsage = Math.Max(snapshot.CpuUsage, snapshot.GpuUsage);
                if (maxUsage < 0)
                {
                    powerLoad = maxTemp >= 75 ? "Heavy Load" : maxTemp >= 55 ? "Light Load" : "Idle";
                    loadString = "Unknown";
                }
                else
                {
                    powerLoad = maxUsage >= 70 ? "Heavy Load" : maxUsage >= 30 ? "Light Load" : "Idle";
                    loadString = $"{maxUsage:F0}%";
                }
            }
            else
            {
                powerLoad = maxPower >= 65 ? "Heavy Load" : maxPower >= 40 ? "Light Load" : "Idle";
                loadString = $"{maxPower:F0}W";
            }

            string decision = "Maintaining Silent Profile";
            if (thermalState == "Critical") decision = "Max Cooling Override";
            else if (thermalState == "Hot" && powerLoad == "Heavy Load") decision = "Aggressive Cooling";
            else if (thermalState == "Hot" && powerLoad == "Light Load") decision = "Slight Ramp Up";
            else if (thermalState == "Hot" && powerLoad == "Idle") decision = "Ignoring Harmless Spike";
            else if (thermalState == "Warm" && powerLoad == "Heavy Load") decision = "Slight Ramp Up";
            else if (thermalState == "Warm" && powerLoad == "Light Load") decision = "Maintaining Profile";
            else if (thermalState == "Warm" && powerLoad == "Idle") decision = "Maintaining Silent Profile";
            
            return ($"{thermalState} ({maxTemp:F0}°C)", $"{powerLoad} ({loadString})", decision);
        }
    }
}
