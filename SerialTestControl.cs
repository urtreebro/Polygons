using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using Polygons.Models;
using Avalonia;
using Avalonia.Controls;

namespace Polygons;

public class SerialTestControl : UserControl
{
    private readonly List<Shape> _shapes = [];

    public override void Render(DrawingContext context)
    {
        foreach (var shape in _shapes)
        {
            shape.Draw(context);
        }

        if (_shapes.Count >= 3)
        {
            SerialDraw(context);
        }

        RemoveShapesInsideHull();
    }

    public async void Test()
    {
        var rnd = new Random();
        for (int i = 0; i < 100; ++i)
        {
            for (int j = 0; j < 500; ++j)
            {
                _shapes.Add(new Circle(rnd.Next(1, 1000), rnd.Next(1, 500)));
            }
            InvalidateVisual();
            await Task.Delay(1000);
        }
    }

    private void RemoveShapesInsideHull()
    {
        foreach (var shape in _shapes.ToList().Where(shape => !shape.IsInConvexHull))
        {
            _shapes.Remove(shape);
        }
    }

    private void SerialDraw(DrawingContext context)
    {
        foreach (var shape in _shapes)
        {
            shape.IsInConvexHull = false;
        }

        int i = 0;
        foreach (var s1 in _shapes)
        {
            int j = 0;
            foreach (var s2 in _shapes)
            {
                if (j <= i)
                {
                    j++;
                    continue;
                }

                int l = 0;
                bool upper = false, lower = false;
                double k = (double)(s2.Y - s1.Y) / (s2.X - s1.X);
                double b = s2.Y - k * s2.X;
                foreach (var s3 in _shapes)
                {
                    if ((s3.X == s2.X && s3.Y == s2.Y) || (s3.X == s1.X && s3.Y == s1.Y))
                    {
                        l++;
                        continue;
                    }

                    if (l != i && l != j)
                    {
                        if (s1.X != s2.X)
                        {
                            if (s3.X * k + b > s3.Y)
                            {
                                lower = true;
                            }
                            else if (s3.X * k + b < s3.Y)
                            {
                                upper = true;
                            }
                        }
                        else
                        {
                            if (s2.X > s3.X)
                            {
                                lower = true;
                            }
                            else if (s2.X < s3.X)
                            {
                                upper = true;
                            }
                        }
                    }

                    l++;
                }

                if (upper != lower)
                {
                    Brush lineBrush = new SolidColorBrush(Colors.BlueViolet);
                    Pen pen = new(lineBrush, lineCap: PenLineCap.Square);
                    var point1 = new Point(s1.X, s1.Y);
                    var point2 = new Point(s2.X, s2.Y);
                    s1.IsInConvexHull = true;
                    s2.IsInConvexHull = true;
                    context.DrawLine(pen, point1, point2);
                }

                j++;
            }

            i++;
        }
    }
}