using System;
using LenovoLegionToolkit.Lib.Messaging.Messages;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public class SmartAutoTelemetryMessage : IMessage
    {
        public string ThermalState { get; }
        public string PowerLoad { get; }
        public string Decision { get; }
        public string OutputState { get; }

        public SmartAutoTelemetryMessage(string thermalState, string powerLoad, string decision, string outputState)
        {
            ThermalState = thermalState;
            PowerLoad = powerLoad;
            Decision = decision;
            OutputState = outputState;
        }
    }
}
