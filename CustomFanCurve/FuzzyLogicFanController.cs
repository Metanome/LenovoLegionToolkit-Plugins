using System;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public static class FuzzyLogicFanController
    {
        // Target outputs (%)
        private const int OutputSilent = 15;
        private const int OutputMaintain = -1; // Special flag to ignore spikes
        private const int OutputSlightRamp = 40;
        private const int OutputAggressive = 70;
        private const int OutputMax = 100;

        /// <param name="fanIndex">Zero-based index into AvailableFanIds (0=first/CPU fan, 1=second/GPU fan, 2+=aux)</param>
        public static int CalculateSmartRpm(HardwareSensorSnapshot snapshot, int maxRpm, int currentTargetRpm, int fanIndex)
        {
            // Safety overrides
            if (snapshot.CpuTemp >= 85 || snapshot.GpuTemp >= 85) return maxRpm;

            // BUG-07: use positional fanIndex (0=CPU, 1=GPU) not hardware fanId value
            float temp = snapshot.CpuTemp;
            float power = snapshot.CpuPower;

            if (fanIndex == 1)
            {
                temp = snapshot.GpuTemp;
                power = snapshot.GpuPower;
            }
            else if (fanIndex > 1)
            {
                temp = Math.Max(snapshot.CpuTemp, snapshot.GpuTemp);
                power = Math.Max(snapshot.CpuPower, snapshot.GpuPower);
            }

            // Fuzzification
            bool isHot = temp >= 65;
            bool isWarm = temp >= 45 && temp < 65;
            bool isCold = temp < 45;

            bool isHeavyLoad;
            bool isLightLoad;
            bool isIdle;

            if (power < 0)
            {
                // Fallback to usage
                float usage = fanIndex == 1 ? snapshot.GpuUsage : (fanIndex == 0 ? snapshot.CpuUsage : Math.Max(snapshot.CpuUsage, snapshot.GpuUsage));
                if (usage < 0)
                {
                    isHeavyLoad = temp >= 75;
                    isLightLoad = temp >= 55 && temp < 75;
                    isIdle = temp < 55;
                }
                else
                {
                    isHeavyLoad = usage >= 70;
                    isLightLoad = usage >= 30 && usage < 70;
                    isIdle = usage < 30;
                }
            }
            else
            {
                isHeavyLoad = power >= 45;
                isLightLoad = power >= 20 && power < 45;
                isIdle = power < 20;
            }

            // Rule Evaluation Matrix
            int targetPercent = OutputSilent;

            if (isHot)
            {
                if (isHeavyLoad) targetPercent = OutputAggressive;
                else if (isLightLoad) targetPercent = OutputSlightRamp;
                else if (isIdle) targetPercent = OutputMaintain; // Ignore micro-spike
            }
            else if (isWarm)
            {
                if (isHeavyLoad) targetPercent = OutputSlightRamp;
                else if (isLightLoad) targetPercent = OutputSilent;
                else if (isIdle) targetPercent = OutputSilent;
            }
            else if (isCold)
            {
                targetPercent = OutputSilent; // Always silent if cold
            }

            // Defuzzification
            if (targetPercent == OutputMaintain)
            {
                if (currentTargetRpm == 0) return 0;
                return currentTargetRpm; 
            }

            return (int)(maxRpm * (targetPercent / 100.0));
        }

        public static (string ThermalState, string PowerLoad, string Decision) GetGlobalTelemetryStrings(HardwareSensorSnapshot snapshot)
        {
            float maxTemp = Math.Max(snapshot.CpuTemp, snapshot.GpuTemp);
            float maxPower = Math.Max(snapshot.CpuPower, snapshot.GpuPower);

            string thermalState = maxTemp >= 85 ? "Critical" : maxTemp >= 65 ? "Hot" : maxTemp >= 45 ? "Warm" : "Cold";
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
                powerLoad = maxPower >= 45 ? "Heavy Load" : maxPower >= 20 ? "Light Load" : "Idle";
                loadString = $"{maxPower:F0}W";
            }

            string decision = "Maintaining Silent Profile";
            if (thermalState == "Critical") decision = "Max Cooling Override";
            else if (thermalState == "Hot" && powerLoad == "Heavy Load") decision = "Aggressive Cooling";
            else if (thermalState == "Hot" && powerLoad == "Light Load") decision = "Slight Ramp Up";
            else if (thermalState == "Hot" && powerLoad == "Idle") decision = "Ignoring Harmless Spike";
            else if (thermalState == "Warm" && powerLoad == "Heavy Load") decision = "Slight Ramp Up";
            else if (thermalState == "Warm" && powerLoad == "Light Load") decision = "Maintaining Profile";
            else if (thermalState == "Warm" && powerLoad == "Idle") decision = "Maintaining Silent Profile"; // BUG-17: explicit case
            
            return ($"{thermalState} ({maxTemp:F0}°C)", $"{powerLoad} ({loadString})", decision);
        }
    }
}
