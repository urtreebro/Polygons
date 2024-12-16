using System;
using Avalonia;
using Avalonia.Media;

namespace Polygons.Models;

public sealed class Triangle : Shape
{
    public Triangle(int x, int y, Color color) : base(x, y, color) { }

    public override bool IsInside(int nx, int ny)
    {
        throw new NotImplementedException();
    }

    public override void Draw(DrawingContext context)
    {
        Brush lineBrush = new SolidColorBrush(color);
        Pen pen = new(lineBrush, 2, lineCap: PenLineCap.Square);
        var point1 = new Point(x, y - r);
        var point2 = new Point(x - r * (float)Math.Sqrt(3) / 2, y + (float)r / 2);
        var point3 = new Point(x + r * (float)Math.Sqrt(3) / 2, y + (float)r / 2);
        context.DrawLine(pen, point1, point2);
        context.DrawLine(pen, point1, point3);
        context.DrawLine(pen, point2, point3);
    }
}