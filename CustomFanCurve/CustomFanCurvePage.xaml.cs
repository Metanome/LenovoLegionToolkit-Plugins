using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Messaging;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.Plugin.CustomFanCurve.Resources;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public partial class CustomFanCurvePage : Page
    {
        private CustomFanCurveService _controlService;
        private CustomFanCurveConfigManager _configManager;
        private ICustomFanMonitoringService _monitoring;
        private IReadOnlyList<int> _fanIds = System.Array.Empty<int>();
        private readonly List<CustomFanCurveControlV3> _fanControls = new();
        private MachineInformation _machineInfo;

        public CustomFanCurvePage()
        {
            var realCulture = LenovoLegionToolkit.Lib.Resources.Resource.Culture ?? System.Threading.Thread.CurrentThread.CurrentUICulture;
            this.Language = System.Windows.Markup.XmlLanguage.GetLanguage(realCulture.IetfLanguageTag);
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _controlService?.OnUIClosed();
            MessagingCenter.Unsubscribe<SmartAutoTelemetryMessage>(this);
            if (_configManager != null) _configManager.SettingsChanged -= UpdateDashboardVisibility;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var provider = Runtime.Provider as CustomFanCurveProvider;
            if (provider == null) return;

            _controlService = provider.ControlService;
            _configManager = provider.ConfigManager;
            _monitoring = provider.Monitoring;
            _fanIds = provider.AvailableFanIds;

            _machineInfo = await Compatibility.GetMachineInformationAsync().ConfigureAwait(true);

            _loadingOverlay.Visibility = Visibility.Visible;

            await _controlService.InitializationTask;
            _fanIds = provider.AvailableFanIds;
            _controlService.OnUIOpened();
            await LoadFanControlsAsync();
            _loadingOverlay.Visibility = Visibility.Collapsed;
            _enableCustomFanToggle.IsChecked = _configManager.Settings.IsCustomFanEnabled;
            
            _configManager.SettingsChanged += UpdateDashboardVisibility;
            MessagingCenter.Subscribe<SmartAutoTelemetryMessage>(this, OnSmartAutoTelemetry);
            UpdateDashboardVisibility();
        }

        private void UpdateDashboardVisibility()
        {
            Dispatcher.InvokeAsync(() => 
            {
                bool isSmartAuto = _configManager.Settings.IsSmartAutoEnabled;
                bool isCustomFanEnabled = _configManager.Settings.IsCustomFanEnabled;

                _smartAutoDashboard.Visibility = isSmartAuto ? Visibility.Visible : Visibility.Collapsed;
                _smartAutoDashboard.Opacity = isCustomFanEnabled ? 1.0 : 0.5;

                _fanControlStackPanel.Visibility = isSmartAuto ? Visibility.Collapsed : Visibility.Visible;
                _fanControlStackPanel.IsEnabled = isCustomFanEnabled;

                _fanSelector.IsEnabled = isCustomFanEnabled && !isSmartAuto;
                _addNodeButton.IsEnabled = isCustomFanEnabled && !isSmartAuto;

                if (isSmartAuto && !isCustomFanEnabled)
                {
                    _dashboardThermalState.Text = "-";
                    _dashboardPowerLoad.Text = "-";
                    _dashboardDecision.Text = "Custom Fan Curve Disabled";
                    _dashboardOutput.Text = "-";

                    var defaultBrush = (System.Windows.Media.Brush)FindResource("TextFillColorPrimaryBrush");
                    _dashboardIcon.Foreground = (System.Windows.Media.Brush)FindResource("AccentTextFillColorPrimaryBrush");
                    _dashboardTitle.Foreground = defaultBrush;
                    _dashboardThermalState.Foreground = defaultBrush;
                    _dashboardPowerLoad.Foreground = defaultBrush;
                    _dashboardDecision.Foreground = defaultBrush;
                    _dashboardOutput.Foreground = defaultBrush;
                }
            });
        }

        private void OnSmartAutoTelemetry(SmartAutoTelemetryMessage msg)
        {
            Dispatcher.InvokeAsync(() => 
            {
                _dashboardThermalState.Text = msg.ThermalState;
                System.Windows.Media.Brush thermalBrush;
                if (msg.ThermalState.StartsWith("Critical")) thermalBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 69, 58));
                else if (msg.ThermalState.StartsWith("Hot")) thermalBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 159, 10));
                else if (msg.ThermalState.StartsWith("Warm")) thermalBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 214, 10));
                else thermalBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(48, 209, 88));
                _dashboardThermalState.Foreground = thermalBrush;
                _dashboardIcon.Foreground = thermalBrush;
                _dashboardTitle.Foreground = thermalBrush;

                _dashboardPowerLoad.Text = msg.PowerLoad;
                if (msg.PowerLoad.StartsWith("Heavy")) _dashboardPowerLoad.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 69, 58));
                else if (msg.PowerLoad.StartsWith("Light")) _dashboardPowerLoad.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 159, 10));
                else _dashboardPowerLoad.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(48, 209, 88));

                _dashboardDecision.Text = msg.Decision;
                _dashboardDecision.Foreground = (System.Windows.Media.Brush)FindResource("AccentTextFillColorPrimaryBrush");

                _dashboardOutput.Text = msg.OutputState;
                _dashboardOutput.Foreground = (System.Windows.Media.Brush)FindResource("TextFillColorPrimaryBrush");
            });
        }

        private async Task LoadFanControlsAsync()
        {
            _fanControlStackPanel.Children.Clear(); _fanControls.Clear(); _fanSelector.Items.Clear();

            for (int i = 0; i < _fanIds.Count; i++)
            {
                var fanId = _fanIds[i];
                var entry = _configManager.GetEntry(fanId);
                if (entry == null)
                {
                    entry = new CustomFanCurveEntry { FanId = fanId };
                    _configManager.SaveEntry(entry);
                }

                if (entry.CurveNodes.Count == 0) continue;

                var vm = new CustomFanCurveControlViewModel(entry, _configManager, _monitoring);
                var control = new CustomFanCurveControlV3();
                control.SetViewModel(vm);
                control.Tag = fanId;
                control.Visibility = Visibility.Collapsed;
                _fanControls.Add(control);
                _fanControlStackPanel.Children.Add(control);

                string fanName;
                if (i == 0) fanName = Resource.CpuFan;
                else if (i == 1) fanName = Resource.GpuFan;
                else fanName = Resource.SystemFan;

                _fanSelector.Items.Add(fanName);
            }
            if (_fanSelector.Items.Count > 0) _fanSelector.SelectedIndex = 0;
        }

        private void FanSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < _fanControls.Count; i++)
                _fanControls[i].Visibility = i == _fanSelector.SelectedIndex ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EnableCustomFan_Changed(object sender, RoutedEventArgs e)
        {
            if (_enableCustomFanToggle.IsLoaded)
                _ = _controlService.SetCustomFanEnabled(_enableCustomFanToggle.IsChecked ?? false);
        }

        private async void MaxFan_Click(object sender, RoutedEventArgs e)
        {
            var current = _configManager.Settings.IsFullSpeed;
            await _controlService.SetFullSpeed(!current);
            UpdateMaxFanButtonState(!current);
        }

        private void UpdateMaxFanButtonState(bool isFullSpeed)
        {
            if (isFullSpeed)
            {
                var sp = new StackPanel { Orientation = Orientation.Horizontal };
                sp.Children.Add(new Wpf.Ui.Controls.SymbolIcon { Symbol = Wpf.Ui.Common.SymbolRegular.Flash24, Margin = new Thickness(0, 0, 6, 0), FontSize = 15, Foreground = System.Windows.Media.Brushes.OrangeRed });
                sp.Children.Add(new TextBlock { Text = Resource.FullSpeedActive, FontSize = 13, FontWeight = FontWeights.SemiBold, Foreground = System.Windows.Media.Brushes.OrangeRed, VerticalAlignment = VerticalAlignment.Center });
                _maxFanButton.Content = sp;
                _maxFanButton.ToolTip = Resource.RecoverCurve;
            }
            else
            {
                var sp = new StackPanel { Orientation = Orientation.Horizontal };
                sp.Children.Add(new Wpf.Ui.Controls.SymbolIcon { Symbol = Wpf.Ui.Common.SymbolRegular.Flash24, Margin = new Thickness(0, 0, 6, 0), FontSize = 15 });
                sp.Children.Add(new TextBlock { Text = Resource.MaxSpeed, VerticalAlignment = VerticalAlignment.Center });
                _maxFanButton.Content = sp;
                _maxFanButton.ToolTip = Resource.MaxSpeedTooltip;
            }
        }

        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            foreach (var control in _fanControls)
            {
                if (control.Visibility == Visibility.Visible && control.DataContext is CustomFanCurveControlViewModel vm)
                { vm.AddPointCommand.Execute(null); break; }
            }
        }

        private void GlobalSettings_Click(object sender, RoutedEventArgs e)
        {
            var series = _machineInfo.LegionSeries;
            var isLegion = series != LegionSeries.ThinkBook && series != LegionSeries.Lenovo_Slim
                && series != LegionSeries.IdeaPad
                && series != LegionSeries.YOGA
                && series != LegionSeries.Motorola && series != LegionSeries.Unknown;
            var isITSMode = _machineInfo.Properties.SupportsITSMode;
            var window = new GlobalSettingsWindow(_configManager, isLegion, isITSMode)
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.ShowDialog();
        }
    }
}
