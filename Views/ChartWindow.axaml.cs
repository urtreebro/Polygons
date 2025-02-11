using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Polygons.Views;

public partial class ChartWindow : Window
{
    private readonly Tuple<int, int>[]? _jarvisChart;
    private readonly Tuple<int, int>[]? _byDefChart;
    
    public ChartWindow(Tuple<int, int>[]? jarv, Tuple<int, int>[]? def)
    {
        InitializeComponent();
        _jarvisChart = jarv;
        _byDefChart = def;
    }

    private void MyChartControl_OnLoaded(object? sender, RoutedEventArgs e)
    {
        ChartControl? chartControl = this.Find<ChartControl>("MyChartControl");
        chartControl?.SetArrays(_jarvisChart, _byDefChart);
    }
}