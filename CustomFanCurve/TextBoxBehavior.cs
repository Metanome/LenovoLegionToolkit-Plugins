using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve
{
    public static class TextBoxBehavior
    {
        public static readonly DependencyProperty NumericOnlyProperty =
            DependencyProperty.RegisterAttached(
                "NumericOnly",
                typeof(bool),
                typeof(TextBoxBehavior),
                new PropertyMetadata(false, OnNumericOnlyChanged));

        public static bool GetNumericOnly(DependencyObject obj) => (bool)obj.GetValue(NumericOnlyProperty);
        public static void SetNumericOnly(DependencyObject obj, bool value) => obj.SetValue(NumericOnlyProperty, value);

        private static void OnNumericOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox tb) return;

            tb.PreviewTextInput -= OnPreviewTextInput;
            DataObject.RemovePastingHandler(tb, OnPasting);

            if ((bool)e.NewValue)
            {
                tb.PreviewTextInput += OnPreviewTextInput;
                DataObject.AddPastingHandler(tb, OnPasting);
            }
        }

        private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9.]+$");
        }

        private static void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var text = (string)e.DataObject.GetData(DataFormats.Text);
                if (Regex.IsMatch(text, "^[0-9.]+$")) return;
            }
            e.CancelCommand();
        }
    }
}
