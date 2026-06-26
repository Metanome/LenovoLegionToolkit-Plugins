using System;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public partial class CustomFanCurvePage : Wpf.Ui.Controls.UiPage
    {
        private CustomFanCurvePageViewModel? _viewModel;
        private CustomFanCurveConfigManager? _configManager;
        private MachineInformation _machineInfo;

        public CustomFanCurvePage()
        {
            var realCulture = LenovoLegionToolkit.Lib.Resources.Resource.Culture ?? System.Threading.Thread.CurrentThread.CurrentUICulture;
            this.Language = System.Windows.Markup.XmlLanguage.GetLanguage(realCulture.IetfLanguageTag);
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Runtime.Provider is not CustomFanCurveProvider provider) return;

            _configManager = provider.ConfigManager;
            provider.ControlService.OnUIOpened();

            _machineInfo = await Compatibility.GetMachineInformationAsync().ConfigureAwait(true);

            _viewModel = new CustomFanCurvePageViewModel(_configManager, provider.ControlService, provider.Monitoring);
            DataContext = _viewModel;

            await provider.ControlService.InitializationTask;
            
            if (!this.IsLoaded)
            {
                return;
            }
            
            _viewModel.LoadFans(provider.AvailableFanIds);
            
            _viewModel.IsProbing = false;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (Runtime.Provider is CustomFanCurveProvider provider)
            {
                provider.ControlService?.OnUIClosed();
            }
            _viewModel?.Detach();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (_configManager == null) return;

            var series = _machineInfo.LegionSeries;
            var isLegion = series != LegionSeries.ThinkBook && series != LegionSeries.Lenovo_Slim
                && series != LegionSeries.IdeaPad
                && series != LegionSeries.YOGA
                && series != LegionSeries.Motorola && series != LegionSeries.Unknown;
            var isITSMode = _machineInfo.Properties.SupportsITSMode;
            
            var window = new SettingsWindow(_configManager, isLegion, isITSMode)
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.ShowDialog();
        }
    }
}
