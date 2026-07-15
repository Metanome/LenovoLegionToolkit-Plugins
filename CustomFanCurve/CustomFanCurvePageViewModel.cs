using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using LenovoLegionToolkit.Lib.Messaging;
using LenovoLegionToolkit.Plugin.CustomFanCurve.Resources;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public class CustomFanCurvePageViewModel : INotifyPropertyChanged
    {
        private readonly CustomFanCurveConfigManager _configManager;
        private readonly CustomFanCurveService _controlService;
        private readonly ICustomFanMonitoringService _monitoring;
        private readonly LenovoLegionToolkit.Lib.Settings.ApplicationSettings? _appSettings;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<CustomFanCurveControlViewModel> FanViewModels { get; } = new();

        private static int _lastSelectedFanId = 1;

        public bool IsHardwareSensorsEnabled => _appSettings?.Store.EnableHardwareSensors ?? false;
        public bool IsHardwareSensorsWarningVisible => !IsHardwareSensorsEnabled;

        public string HardwareSensorsWarningDescription => LenovoLegionToolkit.Lib.Utils.PawnIOHelper.IsPawnIOInstalled()
            ? (Resource.ResourceManager.GetString("HardwareSensorsWarningDescription") ?? "Hardware sensors background polling must be enabled in settings for Custom Fan Curve to function.")
            : (Resource.ResourceManager.GetString("PawnIOWarningDescription") ?? "You need to install PawnIO first to use hardware sensors!");

        private CustomFanCurveControlViewModel? _selectedFanViewModel;
        public CustomFanCurveControlViewModel? SelectedFanViewModel
        {
            get => _selectedFanViewModel;
            set
            {
                _selectedFanViewModel = value;
                OnPropertyChanged();
                if (value != null)
                {
                    _lastSelectedFanId = value.FanId;
                }
            }
        }

        public bool IsCustomFanEnabled
        {
            get => _configManager?.Settings.IsCustomFanEnabled ?? false;
            set
            {
                if (_controlService != null && IsCustomFanEnabled != value)
                {
                    _ = _controlService.SetCustomFanEnabled(value);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsSettingsEnabled));
                }
            }
        }

        private bool _isProbing = true;
        public bool IsProbing
        {
            get => _isProbing;
            set
            {
                if (_isProbing != value)
                {
                    _isProbing = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsUiEnabled));
                }
            }
        }

        public bool IsUiEnabled => !_isProbing;

        public bool IsSmartAutoEnabled => _configManager?.Settings.IsSmartAutoEnabled ?? false;
        public bool IsSettingsEnabled => IsCustomFanEnabled && !IsSmartAutoEnabled;

        public bool IsFullSpeed
        {
            get => _configManager?.Settings.IsFullSpeed ?? false;
            set
            {
                if (_controlService != null && IsFullSpeed != value)
                {
                    _ = _controlService.SetFullSpeed(value);
                    OnPropertyChanged();
                }
            }
        }

        private string _dashboardThermalState = "-";
        public string DashboardThermalState { get => _dashboardThermalState; set { _dashboardThermalState = value; OnPropertyChanged(); } }

        private string _dashboardThermalStateRaw = "-";
        public string DashboardThermalStateRaw { get => _dashboardThermalStateRaw; set { _dashboardThermalStateRaw = value; OnPropertyChanged(); } }

        private string _dashboardPowerLoad = "-";
        public string DashboardPowerLoad { get => _dashboardPowerLoad; set { _dashboardPowerLoad = value; OnPropertyChanged(); } }

        private string _dashboardPowerLoadRaw = "-";
        public string DashboardPowerLoadRaw { get => _dashboardPowerLoadRaw; set { _dashboardPowerLoadRaw = value; OnPropertyChanged(); } }

        private string _dashboardDecision = "-";
        public string DashboardDecision { get => _dashboardDecision; set { _dashboardDecision = value; OnPropertyChanged(); } }

        private string _dashboardOutput = "-";
        public string DashboardOutput { get => _dashboardOutput; set { _dashboardOutput = value; OnPropertyChanged(); } }

        public ICommand ToggleFullSpeedCommand { get; }
        public ICommand AddNodeCommand { get; }

        public CustomFanCurvePageViewModel() { }

        internal CustomFanCurvePageViewModel(CustomFanCurveConfigManager configManager, CustomFanCurveService controlService, ICustomFanMonitoringService monitoring)
        {
            _configManager = configManager;
            _controlService = controlService;
            _monitoring = monitoring;
            try
            {
                _appSettings = LenovoLegionToolkit.Lib.IoCContainer.Resolve<LenovoLegionToolkit.Lib.Settings.ApplicationSettings>();
            }
            catch { /* Ignore */ }

            ToggleFullSpeedCommand = new RelayCommand(() => IsFullSpeed = !IsFullSpeed);
            AddNodeCommand = new RelayCommand(() => SelectedFanViewModel?.AddNodeCommand.Execute(null));

            _configManager.SettingsChanged += OnSettingsChanged;
            MessagingCenter.Subscribe<SmartAutoTelemetryMessage>(this, OnSmartAutoTelemetry);

            OnSettingsChanged();
        }

        public void LoadFans(System.Collections.Generic.IReadOnlyList<int> fanIds)
        {
            foreach (var vm in FanViewModels)
            {
                vm.Detach();
            }
            FanViewModels.Clear();
            for (int i = 0; i < fanIds.Count; i++)
            {
                var fanId = fanIds[i];
                var entry = _configManager.GetEntry(fanId);
                if (entry == null)
                {
                    entry = new CustomFanCurveEntry { FanId = fanId };
                    _configManager.SaveEntry(entry);
                }
                if (entry.CurveNodes.Count > 0)
                {
                    string displayName = i switch
                    {
                        0 => Resource.CpuFan,
                        1 => Resource.GpuFan,
                        _ => fanIds.Count > 3 ? $"{Resource.SystemFan} {i - 1}" : Resource.SystemFan
                    };
                    FanViewModels.Add(new CustomFanCurveControlViewModel(entry, _configManager, _monitoring, displayName));
                }
            }
            if (FanViewModels.Count > 0)
            {
                var selected = FanViewModels[0];
                foreach (var vm in FanViewModels)
                {
                    if (vm.FanId == _lastSelectedFanId)
                    {
                        selected = vm;
                        break;
                    }
                }
                SelectedFanViewModel = selected;
            }
        }

        private void OnSettingsChanged()
        {
            OnPropertyChanged(nameof(IsCustomFanEnabled));
            OnPropertyChanged(nameof(IsSmartAutoEnabled));
            OnPropertyChanged(nameof(IsFullSpeed));
            OnPropertyChanged(nameof(IsSettingsEnabled));
            if (IsSmartAutoEnabled && !IsCustomFanEnabled)
            {
                DashboardThermalState = "-";
                DashboardPowerLoad = "-";
                DashboardDecision = "Custom Fan Curve Disabled";
                DashboardOutput = "-";
                DashboardThermalStateRaw = "-";
                DashboardPowerLoadRaw = "-";
            }
        }

        private void OnSmartAutoTelemetry(SmartAutoTelemetryMessage msg)
        {
            DashboardThermalState = msg.ThermalState;
            DashboardPowerLoad = msg.PowerLoad;
            DashboardDecision = msg.Decision;
            DashboardOutput = msg.OutputState;
            DashboardThermalStateRaw = msg.ThermalState.Split(' ')[0];
            DashboardPowerLoadRaw = msg.PowerLoad.Split(' ')[0];
        }

        public void Detach()
        {
            MessagingCenter.Unsubscribe<SmartAutoTelemetryMessage>(this);
            if (_configManager != null) _configManager.SettingsChanged -= OnSettingsChanged;
            foreach (var vm in FanViewModels)
            {
                vm.Detach();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
