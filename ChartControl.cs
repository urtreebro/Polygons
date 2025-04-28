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
    private const int Oy = 550;
    private const int Ox = 30;

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
        context.DrawLine(pen, new Point(Ox, Oy), new Point(530, Oy));
        context.DrawLine(pen, new Point(Ox, Oy), new Point(Ox, 50));

        if (_isChart)
        {
            Tuple<int, double>[][] charts;
            switch (_chartToDraw)
            {
                case 1:
                    charts = MakeCoordinateSystem([_byDefChart!, CreateFuncChart(i => i * i * i)]);
                    DrawChart(context, charts[0], Colors.DarkOrange);
                    DrawChart(context, charts[1], Colors.Brown);
                    break;
                case 2:
                    charts = MakeCoordinateSystem([
                        _jarvisChart!, CreateFuncChart(i => i * i)
                    ]);
                    DrawChart(context, charts[0], Colors.Blue);
                    DrawChart(context, charts[1], Colors.Cyan);
                    break;
                case 3:
                    charts = MakeCoordinateSystem([_jarvisChart!, _byDefChart!]);
                    DrawChart(context, charts[0], Colors.Blue);
                    DrawChart(context, charts[1], Colors.DarkOrange);
                    break;
            }
        }
    }

    private void DrawChart(DrawingContext context, Tuple<int, double>[]? chart, Color color)
    {
        Brush lineBrush = new SolidColorBrush(color);
        Pen pen = new(lineBrush, lineCap: PenLineCap.Square);

        for (int i = 1; i < chart!.Length; ++i)
        {
            var p1 = new Point(chart[i - 1].Item1, chart[i - 1].Item2);
            var p2 = new Point(chart[i].Item1, chart[i].Item2);
            context.DrawLine(pen, p1, p2);
        }
    }

    private Tuple<int, double>[] CreateFuncChart(Func<int, double> func)
    {
        Tuple<int, double>[] chart = new Tuple<int, double>[50];
        var timer = new Stopwatch();
        var counter = 100;
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
                counter /= 42;
                counter *= 72;
            }

            timer.Stop();
            var elapsed = timer.Elapsed.TotalMilliseconds;
            chart[idx] = new Tuple<int, double>(n, elapsed);
            idx++;
        }

        return chart;
    }

    Tuple<int, double>[][] MakeCoordinateSystem(Tuple<int, double>[][] charts)
    {
        double maxY = double.MinValue;
        foreach (var chart in charts)
        {
            foreach (var p in chart)
            {
                maxY = double.Max(maxY, p.Item2);
            }
        }

        foreach (var chart in charts)
        {
            for (var i = 0; i < chart.Length; ++i)
            {
                chart[i] = new Tuple<int, double>(chart[i].Item1 + Ox, Oy - chart[i].Item2 * 500.0 / maxY);
            }
        }

        return charts;
    }
}