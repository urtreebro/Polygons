using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Polygons;

public class ChartControl : UserControl
{
    private Tuple<int, double>[]? _jarvisChart;
    private Tuple<int, double>[]? _byDefChart;
    private bool _isChart;
    private int _chartToDraw = 3;
    private int _scale = 1000;

    public void SetArrays(Tuple<int, double>[]? jarv, Tuple<int, double>[]? def, int type)
    {
        _jarvisChart = jarv;
        _byDefChart = def;
        _isChart = true;
        _chartToDraw = type;
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
            switch (_chartToDraw)
            {
                case 1:
                    _scale = 1;
                    DrawChart(context, _byDefChart, Colors.DarkOrange);
                    break;
                case 2:
                    _scale = 1000;
                    DrawChart(context, _jarvisChart, Colors.Blue);
                    break;
                case 3:
                    _scale = 1000;
                    DrawChart(context, _jarvisChart, Colors.Blue);
                    DrawChart(context, _byDefChart, Colors.DarkOrange);
                    break;
            }
        }
    }

    private void DrawChart(DrawingContext context, Tuple<int, double>[]? chart, Color color)
    {
        Brush lineBrush = new SolidColorBrush(color);
        Pen pen = new(lineBrush, lineCap: PenLineCap.Square);
        for (int i = 1; i < chart?.Length; ++i)
        {
            var p1 = new Point(chart[i - 1].Item1 + 30, 500 - chart[i - 1].Item2 * _scale);
            var p2 = new Point(chart[i].Item1 + 30, 500 - chart[i].Item2 * _scale);
            context.DrawLine(pen, p1, p2);
        }
    }

    private Tuple<int, double>[] CreateFuncChart(Func<int, double> func)
    {
        Tuple<int, double>[] chart = new Tuple<int, double>[50];
        var timer = new Stopwatch();
        var counter = 0;
        var idx = 0;
        timer.Start();
        for (int n = 1; n <= 500; n += 10)
        {
            if (n == 1)
            {
                for (int x = 0; x < func(n); ++x)
                {
                    counter++;
                }
            }

            timer.Reset();
            timer.Start();
            for (int x = 0; x < func(n); ++x)
            {
                counter++;
            }

            timer.Stop();
            var elapsed = timer.Elapsed.TotalMilliseconds;
            chart[idx] = new Tuple<int, double>(n, elapsed);
            idx++;
        }

        return chart;
    }
}