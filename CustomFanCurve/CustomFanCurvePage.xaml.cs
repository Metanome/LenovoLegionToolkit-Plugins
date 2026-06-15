using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.Plugin.CustomFanCurve.Resources;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public partial class CustomFanCurvePage : Page
    {
        private CustomFanCurveService _controlService;
        private CustomFanCurveConfigManager _configManager;
        private ICustomFanMonitoringService _monitoring;
        private readonly List<CustomFanCurveControlV3> _fanControls = new();
        private MachineInformation _machineInfo;

        public CustomFanCurvePage()
        {
            var realCulture = LenovoLegionToolkit.Lib.Resources.Resource.Culture ?? System.Threading.Thread.CurrentThread.CurrentUICulture;
            this.Language = System.Windows.Markup.XmlLanguage.GetLanguage(realCulture.IetfLanguageTag);
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += (s, e) => _controlService?.OnUIClosed();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var provider = Runtime.Provider as CustomFanCurveProvider;
            if (provider == null) return;

            _controlService = provider.ControlService;
            _configManager = provider.ConfigManager;
            _monitoring = provider.Monitoring;

            _controlService.OnUIOpened();
            await LoadFanControlsAsync();
            _enableCustomFanToggle.IsChecked = _configManager.Settings.IsCustomFanEnabled;
        }

        private async Task LoadFanControlsAsync()
        {
            _fanControlStackPanel.Children.Clear(); _fanControls.Clear(); _fanSelector.Items.Clear();

            _machineInfo = await Compatibility.GetMachineInformationAsync().ConfigureAwait(true);
            var showSystemFan = _machineInfo.LegionSeries == LegionSeries.Legion_Pro_7 && _machineInfo.Generation >= 10;

            var types = new List<FanType> { FanType.Cpu, FanType.Gpu };
            if (showSystemFan) types.Add(FanType.System);

            foreach (var type in types)
            {
                var entry = _configManager.GetEntry(type);
                if (entry == null) continue;

                var vm = new CustomFanCurveControlViewModel(entry, _configManager, _monitoring);
                var control = new CustomFanCurveControlV3();
                control.SetViewModel(vm);
                control.Tag = type.ToString();
                control.Visibility = Visibility.Collapsed;
                _fanControls.Add(control);
                _fanControlStackPanel.Children.Add(control);
                _fanSelector.Items.Add(type.ToString());
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
                _maxFanButton.Content = new TextBlock { Text = $"⚡ {Resource.FullSpeedActive}", FontSize = 13, FontWeight = FontWeights.SemiBold, Foreground = System.Windows.Media.Brushes.OrangeRed };
                _maxFanButton.ToolTip = Resource.RecoverCurve;
            }
            else
            {
                var sp = new StackPanel { Orientation = Orientation.Horizontal };
                sp.Children.Add(new TextBlock { Text = "⚡", Margin = new Thickness(0, 0, 6, 0), FontSize = 14 });
                sp.Children.Add(new TextBlock { Text = Resource.MaxSpeed });
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
                && series != LegionSeries.IdeaPad && series != LegionSeries.IdeaPad_Gaming
                && series != LegionSeries.LOQ && series != LegionSeries.YOGA
                && series != LegionSeries.Motorola && series != LegionSeries.Unknown;
            var window = new GlobalSettingsWindow(_configManager, isLegion);
            window.Owner = Window.GetWindow(this);
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
    }
}
