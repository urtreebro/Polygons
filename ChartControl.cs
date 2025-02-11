using System;
using System.Linq.Expressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Polygons;

public class ChartControl : UserControl
{
    private Tuple<int, int>[]? _jarvisChart;
    private Tuple<int, int>[]? _byDefChart;
    private bool _isChart;

    public void SetArrays(Tuple<int, int>[]? jarv, Tuple<int, int>[]? def)
    {
        _jarvisChart = jarv;
        _byDefChart = def;
        _isChart = true;
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        Brush lineBrush = new SolidColorBrush(Colors.Black);
        Pen pen = new(lineBrush, lineCap: PenLineCap.Square);
        int oy = 500;
        int ox = 30;
        context.DrawLine(pen, new Point(ox, oy), new Point(oy, 500));
        context.DrawLine(pen, new Point(ox, oy), new Point(ox, 50));
        for (int y = 500; y > 50; y -= 50)
        {
            context.DrawLine(pen, new Point(28, y), new Point(32, y));
        }

        for (int x = 30; x < 500; x += 50)
        {
            context.DrawLine(pen, new Point(x, 502), new Point(x, 498));
        }
        if (_isChart)
        {
            DrawChart(context, _jarvisChart, Colors.Blue);
            DrawChart(context, _byDefChart, Colors.DarkOrange);
        }
    }

    private void DrawChart(DrawingContext context, Tuple<int, int>[]? chart, Color color)
    {
        Brush lineBrush = new SolidColorBrush(color);
        Pen pen = new(lineBrush, lineCap: PenLineCap.Square);
        for (int i = 1; i < chart?.Length; ++i)
        {
            var p1 = new Point(chart[i - 1].Item1 + 30, 500 - chart[i - 1].Item2);
            var p2 = new Point(chart[i].Item1 + 30, 500 - chart[i].Item2);
            context.DrawLine(pen, p1, p2);
        }
    }

    private Tuple<int, int>[] CreateFuncChart(Func<int, int> func)
    {
        Tuple<int, int>[] chart = new Tuple<int, int>[50];
        var idx = 0;
        for (int x = 1; x <= 500; x += 10)
        {
            chart[idx] = new Tuple<int, int>(x, func(x));
            idx++;
        }

        return chart;
    }
    
}