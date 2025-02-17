using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Polygons.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Shapes.ItemsSource = new[] { "Circle", "Triangle", "Square" };
        Shapes.SelectedIndex = 1;
        Algorithms.ItemsSource = new[] { "By definition", "Jarvis" };
        Algorithms.SelectedIndex = 0;
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        var point = e.GetCurrentPoint(sender as CustomControl);
        if (point.Properties.IsLeftButtonPressed)
        {
            customControl?.LeftClick((int)e.GetPosition(customControl).X, (int)e.GetPosition(customControl).Y);
        }

        if (point.Properties.IsRightButtonPressed)
        {
            customControl?.RightClick((int)e.GetPosition(customControl).X, (int)e.GetPosition(customControl).Y);
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        customControl?.Move((int)e.GetPosition(customControl).X, (int)e.GetPosition(customControl).Y);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        customControl?.Release((int)e.GetPosition(customControl).X, (int)e.GetPosition(customControl).Y);
    }

    private void Shapes_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");

        int type = Shapes.SelectedIndex;
        customControl?.ChangeType(type);
    }

    private void Button_OnClickParallel(object? sender, RoutedEventArgs e)
    {
        var ownerWindow = this;
        var window = new ParallelTestWindow();
        window.ShowDialog(ownerWindow);
    }

    private void Button_OnClickSerial(object? sender, RoutedEventArgs e)
    {
        var ownerWindow = this;
        var window = new SerialTestWindow();
        window.ShowDialog(ownerWindow);
    }

    private void Algorithms_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");

        int type = Algorithms.SelectedIndex;
        customControl?.ChangeAlgorithm(type);
    }

    private void Button_OnClickCheckPerformance(object? sender, RoutedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");

        var window = new ChartWindow(customControl?.GetChartJarvis(), customControl?.GetChartByDef());
        window.Show();
    }
}