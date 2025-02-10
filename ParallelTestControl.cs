using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Polygons.Models;
using Point = Avalonia.Point;

namespace Polygons;

public class ParallelTestControl : UserControl
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
            ParallelDraw(context);
        }

        RemoveShapesInsideHull();
    }

    public async void Test()
    {
        var rnd = new Random();
        for (int i = 0; i < 100; ++i)
        {
            for (int j = 0; j < 1000; ++j)
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

    private void ParallelDraw(DrawingContext context)
    {
        const double eps = 1e-4;
        foreach (var shape in _shapes)
        {
            shape.IsInConvexHull = false;
        }

        Brush lineBrush = new SolidColorBrush(Colors.BlueViolet);
        Pen pen = new(lineBrush, lineCap: PenLineCap.Square);
        List<Tuple<Point, Point>> toDraw = [];

        Parallel.ForEach(_shapes, s1 =>
        {
            int i = _shapes.IndexOf(s1);
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
                double k = (s2.Y - s1.Y) / (s2.X - s1.X);
                double b = s2.Y - k * s2.X;
                foreach (var s3 in _shapes)
                {
                    if (l != i && l != j)
                    {
                        if (Math.Abs(s1.X - s2.X) > eps)
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

                if (upper != lower || (!upper && !lower))
                {
                    var point1 = new Point(s1.X, s1.Y);
                    var point2 = new Point(s2.X, s2.Y);
                    s1.IsInConvexHull = true;
                    s2.IsInConvexHull = true;
                    toDraw.Add(Tuple.Create(point1, point2));
                }

                j++;
            }
        });

        foreach (var (p1, p2) in toDraw)
        {
            context.DrawLine(pen, p1, p2);
        }
    }
}