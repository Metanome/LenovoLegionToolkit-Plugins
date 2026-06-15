using System;
using System.Linq;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    internal static class CustomFanCurveCalculator
    {
        public static int? Calculate(CustomFanCurveEntry entry, float temperature, int maxRpm)
        {
            var nodes = entry.CurveNodes;
            if (nodes.Count == 0)
            {
                return null;
            }

            var sorted = nodes.OrderBy(n => n.Temperature).ToList();

            double targetPercent;
            if (temperature <= sorted[0].Temperature)
            {
                targetPercent = sorted[0].TargetPercent;
            }
            else if (temperature >= sorted[^1].Temperature)
            {
                targetPercent = sorted[^1].TargetPercent;
            }
            else
            {
                for (var i = 0; i < sorted.Count - 1; i++)
                {
                    if (temperature >= sorted[i].Temperature && temperature <= sorted[i + 1].Temperature)
                    {
                        var ratio = (temperature - sorted[i].Temperature) / (sorted[i + 1].Temperature - sorted[i].Temperature);
                        targetPercent = sorted[i].TargetPercent + ratio * (sorted[i + 1].TargetPercent - sorted[i].TargetPercent);
                        return (int)Math.Round(targetPercent / 100.0 * maxRpm);
                    }
                }

                targetPercent = sorted[^1].TargetPercent;
            }

            return (int)Math.Round(targetPercent / 100.0 * maxRpm);
        }
    }
}
