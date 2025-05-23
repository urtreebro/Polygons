﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Polygons.Models;
using Point = Avalonia.Point;

namespace Polygons;

public class CustomControl : UserControl
{
    private List<Shape> _shapes = [];

    private int _prevX, _prevY;
    private int _shapeType, _algorithmType;

    public List<Shape> Shapes
    {
        get => _shapes;
        set => _shapes = value;
    }

    public bool IsChanged { get; set; }

    public override void Render(DrawingContext context)
    {
        foreach (var shape in _shapes)
        {
            shape.Draw(context);
        }

        if (_shapes.Count >= 3)
        {
            switch (_algorithmType)
            {
                case 0:
                    DrawConvexHullByDef(context);
                    break;
                case 1:
                    DrawConvexHullJarvis(context);
                    break;
            }
        }
    }

    public void LeftClick(int newX, int newY)
    {
        bool inside = false;
        IsChanged = true;
        foreach (var shape in _shapes.Where(shape => shape.IsInside(newX, newY)))
        {
            shape.InitialX = shape.X;
            shape.InitialY = shape.Y;
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

            var actions = new List<Action> { new AddShape(_shapes.Last()) };

            if (_shapes.Count >= 3)
            {
                switch (_algorithmType)
                {
                    case 0:
                        UpdateConvexHullByDef(ref _shapes);
                        break;
                    case 1:
                        UpdateConvexHullJarvis(ref _shapes);
                        break;
                }

                var drag = false;
                if (!_shapes.Last().IsInConvexHull)
                {
                    drag = true;
                    //actions.Add(new RemoveShape(_shapes.Last()));
                    actions.Clear();
                    _shapes.Remove(_shapes.Last());
                    foreach (var shape in _shapes)
                    {
                        shape.IsMoving = true;
                        shape.InitialX = shape.X;
                        shape.InitialY = shape.Y;
                    }

                    _prevX = newX;
                    _prevY = newY;
                }


                if (!drag)
                {
                    actions.AddRange(_shapes.Where(shape => !shape.IsInConvexHull)
                        .Select(shape => new RemoveShape(shape)));
                    RemoveShapesInsideHull();
                }
            }

            if (actions.Count != 0)
            {
                Action.Add(new Combination(actions));
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
            Action.Add(new RemoveShape(shape));
            _shapes.Remove(shape);
            IsChanged = true;
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
        var actions = new List<Action>();
        foreach (var shape in _shapes.Where(shape => shape.IsMoving))
        {
            shape.X += newX - _prevX;
            shape.Y += newY - _prevY;
            if (Math.Abs(shape.InitialX - shape.X) > 1e-6)
            {
                actions.Add(new MoveShape(shape, shape.InitialX, shape.InitialY, shape.X, shape.Y));
            }

            shape.IsMoving = false;
        }

        _prevX = newX;
        _prevY = newY;

        actions.AddRange(_shapes.Where(shape => !shape.IsInConvexHull).Select(shape => new RemoveShape(shape)));
        if (actions.Count != 0)
        {
            Action.Add(new Combination(actions));
        }

        RemoveShapesInsideHull();
        InvalidateVisual();
    }

    private void RemoveShapesInsideHull()
    {
        _shapes.RemoveAll(s => !s.IsInConvexHull);

        InvalidateVisual();
    }

    public void ChangeType(int type)
    {
        _shapeType = type;
    }

    public void ChangeAlgorithm(int type)
    {
        _algorithmType = type;
    }

    private void DrawConvexHullJarvis(DrawingContext context)
    {
        foreach (var shape in _shapes)
        {
            shape.IsInConvexHull = false;
        }

        Brush lineBrush = new SolidColorBrush(Colors.Fuchsia);
        Pen pen = new(lineBrush, lineCap: PenLineCap.Square);
        double minX = Int32.MaxValue;
        double minY = Int32.MinValue;
        Shape first = new Circle(0, 0);
        foreach (var s in _shapes)
        {
            if (s.Y > minY)
            {
                minY = s.Y;
                minX = s.X;
                first = s;
            }
            else if (Math.Abs(s.Y - minY) < 1e-4)
            {
                if (s.X < minX)
                {
                    minY = s.Y;
                    minX = s.X;
                    first = s;
                }
            }
        }

        _shapes.Find(s => s == first)!.IsInConvexHull = true;
        Shape mid = new Circle(first.X - 0.1, first.Y);
        Shape end = mid;
        double maxCos = -2;
        foreach (var s in _shapes)
        {
            if (s == mid || s == first) continue;
            if (maxCos < GetCos(first, mid, s))
            {
                end = s;
                maxCos = GetCos(first, mid, s);
            }
        }

        mid = end;
        _shapes.Find(i => i == end)!.IsInConvexHull = true;
        var p1 = new Point(first.X, first.Y);
        var p2 = new Point(mid.X, mid.Y);
        context.DrawLine(pen, p1, p2);
        var start = first;
        while (true)
        {
            double minCos = 2;
            foreach (var s in _shapes)
            {
                if (s == start || s == mid) continue;
                if (minCos > GetCos(start, mid, s))
                {
                    end = s;
                    minCos = GetCos(start, mid, s);
                }
            }

            start = mid;
            mid = end;
            _shapes.Find(i => i == end)!.IsInConvexHull = true;
            p1 = new Point(start.X, start.Y);
            p2 = new Point(mid.X, mid.Y);
            context.DrawLine(pen, p1, p2);
            if (end == first)
            {
                break;
            }
        }
    }

    private static double GetCos(Shape a, Shape b, Shape c)
    {
        var ba = (a.X - b.X, a.Y - b.Y);
        var bc = (c.X - b.X, c.Y - b.Y);
        var scalarProduct = ba.Item1 * bc.Item1 + ba.Item2 * bc.Item2;
        var baLen = Math.Sqrt(ba.Item1 * ba.Item1 + ba.Item2 * ba.Item2);
        var bcLen = Math.Sqrt(bc.Item1 * bc.Item1 + bc.Item2 * bc.Item2);
        double cos = scalarProduct / (baLen * bcLen);
        return cos;
    }

    private void DrawConvexHullByDef(DrawingContext context)
    {
        const double eps = 1e-4;
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

    private void UpdateConvexHullByDef(ref List<Shape> shapes)
    {
        const double eps = 1e-4;
        foreach (var shape in shapes)
        {
            shape.IsInConvexHull = false;
        }

        int i = 0;
        foreach (var s1 in shapes)
        {
            if (i == shapes.Count - 1)
            {
                break;
            }

            int j = 0;
            foreach (var s2 in shapes)
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
                foreach (var s3 in shapes)
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

    private void UpdateConvexHullJarvis(ref List<Shape> shapes)
    {
        foreach (var shape in shapes)
        {
            shape.IsInConvexHull = false;
        }

        double minX = Int32.MaxValue;
        double minY = Int32.MinValue;
        Shape first = new Circle(0, 0);
        foreach (var s in shapes)
        {
            if (s.Y > minY)
            {
                minY = s.Y;
                minX = s.X;
                first = s;
            }
            else if (Math.Abs(s.Y - minY) < 1e-4)
            {
                if (s.X < minX)
                {
                    minY = s.Y;
                    minX = s.X;
                    first = s;
                }
            }
        }

        shapes.Find(s => s == first)!.IsInConvexHull = true;
        Shape mid = new Circle(first.X - 0.1, first.Y);
        Shape end = mid;
        double maxCos = -2;
        foreach (var s in shapes)
        {
            if (s == mid || s == first) continue;
            if (maxCos < GetCos(first, mid, s))
            {
                end = s;
                maxCos = GetCos(first, mid, s);
            }
        }

        mid = end;
        shapes.Find(i => i == end)!.IsInConvexHull = true;
        var start = first;
        while (true)
        {
            double minCos = 2;
            foreach (var s in shapes)
            {
                if (s == start || s == mid) continue;
                if (minCos > GetCos(start, mid, s))
                {
                    end = s;
                    minCos = GetCos(start, mid, s);
                }
            }

            start = mid;
            mid = end;
            shapes.Find(i => i == end)!.IsInConvexHull = true;
            if (end == first)
            {
                break;
            }
        }
    }

    public Tuple<int, double>[] GetChartJarvis()
    {
        var chart = new Tuple<int, double>[50];
        var rnd = new Random();
        var timer = new Stopwatch();
        List<Shape> shapes = [];
        var idx = 0;
        for (var i = 10; i <= 500; i += 10)
        {
            timer.Reset();
            shapes.Clear();
            for (var j = 0; j < i; ++j)
            {
                shapes.Add(new Circle(rnd.Next(1, 10000), rnd.Next(1, 10000)));
            }

            if (i == 10)
            {
                UpdateConvexHullJarvis(ref shapes);
            }

            timer.Start();
            UpdateConvexHullJarvis(ref shapes);
            timer.Stop();
            var elapsed = timer.Elapsed.TotalMilliseconds;
            chart[idx++] = new Tuple<int, double>(i, elapsed);
        }

        return chart;
    }

    public Tuple<int, double>[] GetChartByDef()
    {
        var chart = new Tuple<int, double>[50];
        var rnd = new Random();
        var timer = new Stopwatch();
        List<Shape> shapes = [];
        var idx = 0;
        for (var i = 10; i <= 500; i += 10)
        {
            timer.Reset();
            shapes.Clear();
            for (var j = 0; j < i; ++j)
            {
                shapes.Add(new Circle(rnd.Next(1, 10000), rnd.Next(1, 10000)));
            }

            if (i == 0)
            {
                UpdateConvexHullByDef(ref shapes);
            }

            timer.Start();
            UpdateConvexHullByDef(ref shapes);
            timer.Stop();
            var elapsed = timer.Elapsed.TotalMilliseconds;
            chart[idx++] = new Tuple<int, double>(i, elapsed);
        }

        return chart;
    }

    public void UpdateRadius(object sender, RadiusEventArgs e)
    {
        Shape.R = e.Radius;
        InvalidateVisual();
    }

    public void UpdateColor(Color color)
    {
        Action.Add(new ChangeColor(Shape.color, color));
        Shape.color = color;
    }

    public void MoveShapes()
    {
        var rnd = new Random();
        foreach (var shape in _shapes)
        {
            shape.X += rnd.Next(-10, 10);
            shape.Y += rnd.Next(-10, 10);
        }

        RemoveShapesInsideHull();
        InvalidateVisual();
    }
}