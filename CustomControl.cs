using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Polygons.Models;
using Point = Avalonia.Point;

namespace Polygons;

public class CustomControl : UserControl
{
    private readonly List<Shape> _shapes = [];

    private int _prevX, _prevY;
    private int _shapeType;

    public override void Render(DrawingContext context)
    {
        foreach (var shape in _shapes)
        {
            shape.Draw(context);
        }

        if (_shapes.Count >= 3)
        {
            DrawConvexHullByDef(context);
        }
    }

    public void LeftClick(int newX, int newY)
    {
        bool inside = false;
        foreach (var shape in _shapes.Where(shape => shape.IsInside(newX, newY)))
        {
            _prevX = newX;
            _prevY = newY;
            shape.IsMoving = true;
            inside = true;
        }

        if (!inside)
        {
            switch (_shapeType)
            {
                case 0:
                    _shapes.Add(new Circle(newX, newY));
                    break;
                case 1:
                    _shapes.Add(new Triangle(newX, newY));
                    break;
                case 2:
                    _shapes.Add(new Square(newX, newY));
                    break;
            }

            if (_shapes.Count >= 3)
            {
                UpdatePointsInConvexHull();
                var drag = false;
                if (!_shapes.Last().IsInConvexHull)
                {
                    drag = true;
                    _shapes.Remove(_shapes.Last());
                    foreach (var shape in _shapes)
                    {
                        shape.IsMoving = true;
                    }

                    _prevX = newX;
                    _prevY = newY;
                }

                if (!drag)
                {
                    RemoveShapesInsideHull();
                }
            }
        }

        InvalidateVisual();
    }

    public void RightClick(int newX, int newY)
    {
        foreach (var shape in _shapes.Where(shape => shape.IsInside(newX, newY)).Reverse())
        {
            _prevX = newX;
            _prevY = newY;
            _shapes.Remove(shape);
            break;
        }

        InvalidateVisual();
    }

    public void Move(int newX, int newY)
    {
        foreach (var shape in _shapes.Where(shape => shape.IsMoving))
        {
            shape.X += newX - _prevX;
            shape.Y += newY - _prevY;
        }

        _prevX = newX;
        _prevY = newY;
        InvalidateVisual();
    }

    public void Release(int newX, int newY)
    {
        foreach (var shape in _shapes.Where(shape => shape.IsMoving))
        {
            shape.X += newX - _prevX;
            shape.Y += newY - _prevY;
            shape.IsMoving = false;
        }

        _prevX = newX;
        _prevY = newY;
        RemoveShapesInsideHull();
        InvalidateVisual();
    }

    private void RemoveShapesInsideHull()
    {
        foreach (var shape in _shapes.ToList().Where(shape => !shape.IsInConvexHull))
        {
            _shapes.Remove(shape);
        }

        InvalidateVisual();
    }

    public void ChangeType(int type)
    {
        _shapeType = type;
    }

    private void DrawConvexHullByDef(DrawingContext context)
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

    private void UpdatePointsInConvexHull()
    {
        foreach (var shape in _shapes)
        {
            shape.IsInConvexHull = false;
        }

        int i = 0;
        foreach (var s1 in _shapes)
        {
            if (i == _shapes.Count - 1)
            {
                break;
            }

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

                if (upper != lower || (upper == false && lower == false))
                {
                    s1.IsInConvexHull = true;
                    s2.IsInConvexHull = true;
                }

                j++;
            }

            i++;
        }
    }
}