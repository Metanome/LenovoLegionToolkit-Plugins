using System.Collections.Generic;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public static class Strings
    {
        private static readonly Dictionary<string, string> Zh = new()
        {
            ["EnableCustomFanCurve"] = "启用自定义风扇曲线",
            ["MaxSpeed"] = "最大转速",
            ["GlobalSettings"] = "全局设置",
            ["LiveMonitor"] = "实时监控",
            ["Temperature"] = "温度",
            ["CurrentSpeed"] = "当前转速",
            ["TargetSpeed"] = "目标转速",
            ["FanSpeed"] = "风扇速度",
            ["AddNodeTooltip"] = "在曲线末尾添加一个新的控制节点",
            ["MaxSpeedTooltip"] = "立刻以最大转速运行所有风扇",
            ["GlobalSettingsTooltip"] = "打开全局设置",
            ["RecoverCurve"] = "点击恢复自定义曲线",
            ["FullSpeedActive"] = "全速中...",
            ["DeleteNode"] = "删除此节点",
            ["TargetPercent"] = "目标转速百分比 (0-100)",
            ["TemperatureUnit"] = "温度值 (°C)",

            ["SettingsTitle"] = "全局设置",
            ["Basic"] = "启停控制",
            ["SensorAndCalc"] = "传感器 & 计算",
            ["SpinUpBoost"] = "加速启动",
            ["ModeSwitch"] = "模式切换",
            ["Advanced"] = "高级",
            ["EnableCurve"] = "启用自定义风扇曲线",
            ["EnableCurveDesc"] = "关闭后将恢复EC自动风扇控制",
            ["ApplyToAllModes"] = "应用到所有性能模式",
            ["ApplyToAllModesDesc"] = "为拯救者设备在任意模式下启用自定义风扇控制",
            ["DebugMode"] = "调试模式",
            ["DebugModeDesc"] = "启用后将输出详细的调试日志",

            ["SensorPolling"] = "传感器轮询",
            ["SensorInterval"] = "传感器轮询间隔 (ms)",
            ["SensorIntervalDesc"] = "从硬件读取温度数据的间隔，默认 500ms",
            ["IgnoreZeroTemp"] = "忽略零温度读数",
            ["IgnoreZeroTempDesc"] = "传感器异常返回 0 时跳过处理",
            ["CalcScheduling"] = "计算调度",
            ["CalcInterval"] = "RPM 计算间隔 (ms)",
            ["CalcIntervalDesc"] = "两次 RPM 计算之间的最小延迟，默认 500ms",
            ["TempDeltaThreshold"] = "温度变化阈值 (°C)",
            ["TempDeltaThresholdDesc"] = "触发 RPM 重算的最小温度变化，默认 0.5°C",

            ["SpinUpDesc"] = "风扇停转后需要较高的初始 RPM 才能克服静摩擦力重新启动。\n启用加速启动后，系统会先短暂以较高 RPM 启动风扇，再降至目标值。",
            ["EnableSpinUp"] = "启用加速启动",
            ["SpinUpRpm"] = "启动 RPM",
            ["SpinUpRpmDesc"] = "加速启动时的临时 RPM 值，默认 3000",
            ["SpinUpDuration"] = "启动持续时间 (ms)",
            ["SpinUpDurationDesc"] = "加速启动 RPM 的持续时间，默认 300ms",

            ["ModeSwitchTitle"] = "电源/散热模式切换行为",
            ["ForceRefreshOnSwitch"] = "模式切换后强制刷新",
            ["ForceRefreshOnSwitchDesc"] = "切换电源模式后重新应用风扇控制",
            ["RefreshCount"] = "刷新次数",
            ["RefreshCountDesc"] = "模式切换后的刷新执行次数，默认 2",
            ["RefreshInterval"] = "刷新间隔 (ms)",
            ["RefreshIntervalDesc"] = "多次刷新之间的延迟，默认 250ms",
            ["ForceRefreshOnEnable"] = "启用时强制刷新",
            ["ClearCacheOnLeave"] = "离开自定义模式时清除缓存",
            ["ClearCacheOnLeaveDesc"] = "清除缓存的 RPM/温度状态",

            ["UiAndMonitoring"] = "UI 与监控",
            ["UiUpdateInterval"] = "UI 更新间隔 (ms)",
            ["UiUpdateIntervalDesc"] = "监控面板数据刷新间隔，默认 1000ms",
            ["UseCachedSnapshot"] = "强制刷新使用缓存快照",
            ["UseCachedSnapshotDesc"] = "而非等待新传感器读数",
            ["WriteEachCycle"] = "全速模式每周期写入",
            ["WriteEachCycleDesc"] = "全速模式下每个计算周期都写入最大 RPM",
            ["ProtectionAndResponse"] = "保护与响应参数",
            ["AccelResponse"] = "加速响应 (RPM/步)",
            ["AccelResponseDesc"] = "每步允许的最大 RPM 上升量，默认 25",
            ["DecelResponse"] = "减速响应 (RPM/步)",
            ["DecelResponseDesc"] = "每步允许的最大 RPM 下降量，默认 12",

            ["Save"] = "保存",
            ["Cancel"] = "取消",
            ["SaveHint"] = "修改将在点击「保存」后立即生效。",
            ["WindowTitle"] = "自定义风扇曲线(WMI)",
            ["SettingsWindowTitle"] = "全局设置 — 自定义风扇曲线(WMI)",
            ["Over50MinRpm"] = "超过时全速运转",
            ["AccelResponseHint"] = "每步最大RPM上升量",
            ["DecelResponseHint"] = "每步最大RPM下降量",
        };

        private static readonly Dictionary<string, string> En = new()
        {
            ["EnableCustomFanCurve"] = "Enable Custom Fan Curve",
            ["MaxSpeed"] = "Max Speed",
            ["GlobalSettings"] = "Global Settings",
            ["LiveMonitor"] = "Live Monitor",
            ["Temperature"] = "Temperature",
            ["CurrentSpeed"] = "Current Speed",
            ["TargetSpeed"] = "Target Speed",
            ["FanSpeed"] = "Fan Speed",
            ["AddNodeTooltip"] = "Add a new control point at the end of the curve",
            ["MaxSpeedTooltip"] = "Run all fans at maximum speed",
            ["GlobalSettingsTooltip"] = "Open global settings",
            ["RecoverCurve"] = "Click to restore custom curve",
            ["FullSpeedActive"] = "Full Speed...",
            ["DeleteNode"] = "Delete this node",
            ["TargetPercent"] = "Target fan speed percentage (0-100)",
            ["TemperatureUnit"] = "Temperature value (°C)",

            ["SettingsTitle"] = "Global Settings",
            ["Basic"] = "Basic",
            ["SensorAndCalc"] = "Sensor & Calc",
            ["SpinUpBoost"] = "Spin-Up Boost",
            ["ModeSwitch"] = "Mode Switch",
            ["Advanced"] = "Advanced",
            ["EnableCurve"] = "Enable Custom Fan Curve",
            ["EnableCurveDesc"] = "Restore BIOS auto fan control when disabled",
            ["ApplyToAllModes"] = "Apply to All Power Modes",
            ["ApplyToAllModesDesc"] = "Enable custom fan control in all power modes on Legion devices",
            ["DebugMode"] = "Debug Mode",
            ["DebugModeDesc"] = "Output detailed debug logs when enabled",

            ["SensorPolling"] = "Sensor Polling",
            ["SensorInterval"] = "Sensor Polling Interval (ms)",
            ["SensorIntervalDesc"] = "Interval for reading temperature data, default 500ms",
            ["IgnoreZeroTemp"] = "Ignore Zero Temperature",
            ["IgnoreZeroTempDesc"] = "Skip processing when sensor returns 0",
            ["CalcScheduling"] = "Calculation Scheduling",
            ["CalcInterval"] = "RPM Calculation Interval (ms)",
            ["CalcIntervalDesc"] = "Minimum delay between two RPM calculations, default 500ms",
            ["TempDeltaThreshold"] = "Temperature Change Threshold (°C)",
            ["TempDeltaThresholdDesc"] = "Minimum temperature change to trigger recalculation, default 0.5°C",

            ["SpinUpDesc"] = "When a fan is stopped, it needs a higher initial RPM to overcome static friction.\nWith spin-up boost enabled, the system briefly runs the fan at a higher RPM before settling to the target.",
            ["EnableSpinUp"] = "Enable Spin-Up Boost",
            ["SpinUpRpm"] = "Spin-Up RPM",
            ["SpinUpRpmDesc"] = "Temporary RPM during spin-up, default 3000",
            ["SpinUpDuration"] = "Spin-Up Duration (ms)",
            ["SpinUpDurationDesc"] = "How long the spin-up RPM lasts, default 300ms",

            ["ModeSwitchTitle"] = "Power/Thermal Mode Switch Behavior",
            ["ForceRefreshOnSwitch"] = "Force Refresh on Mode Switch",
            ["ForceRefreshOnSwitchDesc"] = "Re-apply fan control after switching power modes",
            ["RefreshCount"] = "Refresh Count",
            ["RefreshCountDesc"] = "Number of refresh cycles after mode switch, default 2",
            ["RefreshInterval"] = "Refresh Interval (ms)",
            ["RefreshIntervalDesc"] = "Delay between multiple refreshes, default 250ms",
            ["ForceRefreshOnEnable"] = "Force Refresh on Enable",
            ["ClearCacheOnLeave"] = "Clear Cache When Leaving Custom Mode",
            ["ClearCacheOnLeaveDesc"] = "Clear cached RPM/temperature state",

            ["UiAndMonitoring"] = "UI & Monitoring",
            ["UiUpdateInterval"] = "UI Update Interval (ms)",
            ["UiUpdateIntervalDesc"] = "Monitor panel refresh interval, default 1000ms",
            ["UseCachedSnapshot"] = "Use Cached Snapshot for Forced Refresh",
            ["UseCachedSnapshotDesc"] = "Instead of waiting for new sensor readings",
            ["WriteEachCycle"] = "Write Max Each Cycle in Full Speed",
            ["WriteEachCycleDesc"] = "Write max RPM every calculation cycle in full speed mode",
            ["ProtectionAndResponse"] = "Protection & Response",
            ["AccelResponse"] = "Acceleration Response (RPM/step)",
            ["AccelResponseDesc"] = "Max RPM increase per step, default 25",
            ["DecelResponse"] = "Deceleration Response (RPM/step)",
            ["DecelResponseDesc"] = "Max RPM decrease per step, default 12",

            ["Save"] = "Save",
            ["Cancel"] = "Cancel",
            ["SaveHint"] = "Changes take effect immediately after clicking Save.",
            ["WindowTitle"] = "Custom Fan Curve (WMI)",
            ["SettingsWindowTitle"] = "Global Settings — Custom Fan Curve (WMI)",
            ["Over50MinRpm"] = "Overheat full speed",
            ["AccelResponseHint"] = "Max RPM increase per step",
            ["DecelResponseHint"] = "Max RPM decrease per step",
        };

        private static Dictionary<string, string> _current = Zh;

        public static string Language { get; private set; } = "zh";

        public static void SetLanguage(string lang)
        {
            Language = lang;
            _current = lang == "en" ? En : Zh;
        }

        public static string Get(string key) => _current.TryGetValue(key, out var v) ? v : key;

        public static string EnableCustomFanCurve => Get("EnableCustomFanCurve");
        public static string MaxSpeed => Get("MaxSpeed");
        public static string GlobalSettings => Get("GlobalSettings");
        public static string LiveMonitor => Get("LiveMonitor");
        public static string Temperature => Get("Temperature");
        public static string CurrentSpeed => Get("CurrentSpeed");
        public static string TargetSpeed => Get("TargetSpeed");

        public static string FanSpeed => Get("FanSpeed");
        public static string AddNodeTooltip => Get("AddNodeTooltip");
        public static string MaxSpeedTooltip => Get("MaxSpeedTooltip");
        public static string GlobalSettingsTooltip => Get("GlobalSettingsTooltip");
        public static string RecoverCurve => Get("RecoverCurve");
        public static string FullSpeedActive => Get("FullSpeedActive");
        public static string DeleteNode => Get("DeleteNode");
        public static string TargetPercent => Get("TargetPercent");
        public static string TemperatureUnit => Get("TemperatureUnit");
        public static string SettingsTitle => Get("SettingsTitle");
        public static string Basic => Get("Basic");
        public static string SensorAndCalc => Get("SensorAndCalc");
        public static string SpinUpBoost => Get("SpinUpBoost");
        public static string ModeSwitch => Get("ModeSwitch");
        public static string Advanced => Get("Advanced");
        public static string EnableCurve => Get("EnableCurve");
        public static string EnableCurveDesc => Get("EnableCurveDesc");
        public static string ApplyToAllModes => Get("ApplyToAllModes");
        public static string ApplyToAllModesDesc => Get("ApplyToAllModesDesc");
        public static string DebugMode => Get("DebugMode");
        public static string DebugModeDesc => Get("DebugModeDesc");
        public static string SensorPolling => Get("SensorPolling");
        public static string SensorInterval => Get("SensorInterval");
        public static string SensorIntervalDesc => Get("SensorIntervalDesc");
        public static string IgnoreZeroTemp => Get("IgnoreZeroTemp");
        public static string IgnoreZeroTempDesc => Get("IgnoreZeroTempDesc");
        public static string CalcScheduling => Get("CalcScheduling");
        public static string CalcInterval => Get("CalcInterval");
        public static string CalcIntervalDesc => Get("CalcIntervalDesc");
        public static string TempDeltaThreshold => Get("TempDeltaThreshold");
        public static string TempDeltaThresholdDesc => Get("TempDeltaThresholdDesc");
        public static string SpinUpDesc => Get("SpinUpDesc");
        public static string EnableSpinUp => Get("EnableSpinUp");
        public static string SpinUpRpm => Get("SpinUpRpm");
        public static string SpinUpRpmDesc => Get("SpinUpRpmDesc");
        public static string SpinUpDuration => Get("SpinUpDuration");
        public static string SpinUpDurationDesc => Get("SpinUpDurationDesc");
        public static string ModeSwitchTitle => Get("ModeSwitchTitle");
        public static string ForceRefreshOnSwitch => Get("ForceRefreshOnSwitch");
        public static string ForceRefreshOnSwitchDesc => Get("ForceRefreshOnSwitchDesc");
        public static string RefreshCount => Get("RefreshCount");
        public static string RefreshCountDesc => Get("RefreshCountDesc");
        public static string RefreshInterval => Get("RefreshInterval");
        public static string RefreshIntervalDesc => Get("RefreshIntervalDesc");
        public static string ForceRefreshOnEnable => Get("ForceRefreshOnEnable");
        public static string ClearCacheOnLeave => Get("ClearCacheOnLeave");
        public static string ClearCacheOnLeaveDesc => Get("ClearCacheOnLeaveDesc");
        public static string UiAndMonitoring => Get("UiAndMonitoring");
        public static string UiUpdateInterval => Get("UiUpdateInterval");
        public static string UiUpdateIntervalDesc => Get("UiUpdateIntervalDesc");
        public static string UseCachedSnapshot => Get("UseCachedSnapshot");
        public static string UseCachedSnapshotDesc => Get("UseCachedSnapshotDesc");
        public static string WriteEachCycle => Get("WriteEachCycle");
        public static string WriteEachCycleDesc => Get("WriteEachCycleDesc");
        public static string ProtectionAndResponse => Get("ProtectionAndResponse");
        public static string AccelResponse => Get("AccelResponse");
        public static string AccelResponseDesc => Get("AccelResponseDesc");
        public static string DecelResponse => Get("DecelResponse");
        public static string DecelResponseDesc => Get("DecelResponseDesc");
        public static string Save => Get("Save");
        public static string Cancel => Get("Cancel");
        public static string SaveHint => Get("SaveHint");
        public static string WindowTitle => Get("WindowTitle");
        public static string SettingsWindowTitle => Get("SettingsWindowTitle");
        public static string Over50MinRpm => Get("Over50MinRpm");
        public static string AccelResponseHint => Get("AccelResponseHint");
        public static string DecelResponseHint => Get("DecelResponseHint");
    }
}
