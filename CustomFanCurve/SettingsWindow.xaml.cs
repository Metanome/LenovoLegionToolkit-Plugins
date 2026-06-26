using System.Windows;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public partial class SettingsWindow : UiWindow
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsWindow(CustomFanCurveConfigManager configManager, bool isLegionDevice, bool isITSModeDevice)
        {
            var realCulture = LenovoLegionToolkit.Lib.Resources.Resource.Culture ?? System.Threading.Thread.CurrentThread.CurrentUICulture;
            this.Language = System.Windows.Markup.XmlLanguage.GetLanguage(realCulture.IetfLanguageTag);
            InitializeComponent();
            _viewModel = new SettingsViewModel(configManager, isLegionDevice, isITSModeDevice);
            DataContext = _viewModel;
        }

        private void Save_Click(object sender, RoutedEventArgs e) { _viewModel.SaveAll(); DialogResult = true; Close(); }
        private void Cancel_Click(object sender, RoutedEventArgs e) { DialogResult = false; Close(); }
    }
}
