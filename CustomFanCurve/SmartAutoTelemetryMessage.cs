using System;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public class SmartAutoTelemetryMessage : LenovoLegionToolkit.Lib.Messaging.Messages.IMessage
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
