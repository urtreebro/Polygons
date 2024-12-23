using System;
using Avalonia;
using Avalonia.Media;

namespace Polygons.Models;

public sealed class Triangle : Shape
{
    private Point _point1, _point2, _point3;
    private static double Area => R * R * 0.25 * 3 * Math.Sqrt(3);
    public Triangle(int x = 0, int y = 0) : base(x, y) { }

    public override bool IsInside(int nx, int ny)
    {
        var pointClick = new Point(nx, ny);
        return Math.Abs(Area - HeronFormula(_point1, _point2, pointClick)
                             - HeronFormula(_point1, _point3, pointClick)
                             - HeronFormula(_point2, _point3, pointClick)) <= 0.1;
    }

    public override void Draw(DrawingContext context)
    {
        Brush lineBrush = new SolidColorBrush(color);
        Pen pen = new(lineBrush, 2, lineCap: PenLineCap.Square);
        _point1 = new Point(x, y - R);
        _point2 = new Point(x - R * (float)Math.Sqrt(3) / 2, y + (float)R / 2);
        _point3 = new Point(x + R * (float)Math.Sqrt(3) / 2, y + (float)R / 2);
        context.DrawLine(pen, _point1, _point2);
        context.DrawLine(pen, _point1, _point3);
        context.DrawLine(pen, _point2, _point3);
    }

    private static double HeronFormula(Point p1, Point p2, Point p3)
    {
        var a = Point.Distance(p1, p2);
        var b = Point.Distance(p1, p3);
        var c = Point.Distance(p2, p3);
        var p = (a + b + c) / 2;
        return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
    }
}