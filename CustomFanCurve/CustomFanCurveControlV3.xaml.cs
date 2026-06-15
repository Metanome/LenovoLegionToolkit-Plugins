using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LenovoLegionToolkit.Plugin.CustomFanCurve;

public partial class CustomFanCurveControlV3 : UserControl
{
    private CustomFanCurveControlViewModel? _viewModel;
    private bool _drawRequested;

    public CustomFanCurveControlV3()
    {
        InitializeComponent();
        SizeChanged += (s, e) => RequestDraw();
        Loaded += (s, e) => RequestDraw();
    }

    public void SetViewModel(CustomFanCurveControlViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = _viewModel;
        if (_viewModel != null)
        {
            _viewModel.CurveNodes.CollectionChanged += (s, e) => RequestDraw();
            _viewModel.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(CustomFanCurveControlViewModel.GraphPoints)) RequestDraw(); };
            RequestDraw();
        }
    }

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) => RequestDraw();

    private void RequestDraw()
    {
        if (_drawRequested || _viewModel == null || _canvas.ActualWidth <= 0 || _canvas.ActualHeight <= 0) return;
        _drawRequested = true;
        Dispatcher.InvokeAsync(() => { _drawRequested = false; DrawGraph(); }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
    }

    private void DrawGraph()
    {
        if (_viewModel == null || _canvas.ActualWidth <= 0 || _canvas.ActualHeight <= 0) return;
        var nodes = _viewModel.CurveNodes;
        if (nodes == null || nodes.Count < 2) return;

        var color = Application.Current.Resources["ControlFillColorDefaultBrush"] as SolidColorBrush ?? new SolidColorBrush(Colors.CornflowerBlue);
        _canvas.Children.Clear();

        var sliders = FindVisualChildren<Slider>(_nodeItemsControl).ToList();
        if (sliders.Count == 0 || sliders.Count != nodes.Count) return;

        var points = new List<Point>();
        foreach (var slider in sliders)
        {
            var thumb = FindVisualChild<Thumb>(slider);
            if (thumb is { IsLoaded: true, ActualHeight: > 0 })
                points.Add(thumb.TranslatePoint(new Point(thumb.ActualWidth / 2, thumb.ActualHeight / 2), _canvas));
            else
            {
                var ratio = (slider.Value - slider.Minimum) / (slider.Maximum - slider.Minimum);
                points.Add(slider.TranslatePoint(new Point(slider.ActualWidth / 2, slider.ActualHeight * (1 - ratio)), _canvas));
            }
        }
        if (points.Count < 2) return;

        var gridBrush = new SolidColorBrush(Color.FromArgb(30, color.Color.R, color.Color.G, color.Color.B));
        for (int i = 0; i <= 100; i += 20)
        {
            _canvas.Children.Add(new Line
            {
                X1 = 0, Y1 = _canvas.ActualHeight * (1 - i / 100.0), X2 = _canvas.ActualWidth, Y2 = _canvas.ActualHeight * (1 - i / 100.0),
                Stroke = gridBrush, StrokeThickness = 1, StrokeDashArray = new DoubleCollection { 2, 2 }
            });
        }

        var fig = new PathFigure { StartPoint = points[0] };
        foreach (var pt in points.Skip(1)) fig.Segments.Add(new LineSegment { Point = pt });
        _canvas.Children.Add(new Path { StrokeThickness = 2, Stroke = color, StrokeStartLineCap = PenLineCap.Round, StrokeEndLineCap = PenLineCap.Round, Data = new PathGeometry { Figures = { fig } } });

        var fillPts = new List<Point> { new(points[0].X, _canvas.ActualHeight) };
        fillPts.AddRange(points);
        fillPts.Add(new(points[^1].X, _canvas.ActualHeight));
        _canvas.Children.Add(new Polygon { Fill = new SolidColorBrush(Color.FromArgb(50, color.Color.R, color.Color.G, color.Color.B)), Points = new PointCollection(fillPts) });

        foreach (var pt in points)
        {
            var e = new Ellipse { Width = 10, Height = 10, Fill = color, Stroke = new SolidColorBrush(Colors.White), StrokeThickness = 2 };
            Canvas.SetLeft(e, pt.X - 5); Canvas.SetTop(e, pt.Y - 5);
            _canvas.Children.Add(e);
        }
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject? depObj) where T : DependencyObject
    {
        if (depObj == null) yield break;
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);
            if (child is T t) yield return t;
            foreach (var c in FindVisualChildren<T>(child)) yield return c;
        }
    }

    private static T? FindVisualChild<T>(DependencyObject? parent) where T : DependencyObject
    {
        if (parent == null) return null;
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T t) return t;
            var r = FindVisualChild<T>(child); if (r != null) return r;
        }
        return null;
    }

    private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
    private void TemperatureTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");

    private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(DataFormats.Text) && Regex.IsMatch((string)e.DataObject.GetData(DataFormats.Text), "^[0-9]+$")) return;
        e.CancelCommand();
    }
}
