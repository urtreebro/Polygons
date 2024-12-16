using System;
using Avalonia;
using Avalonia.Media;

namespace Polygons.Models;

public sealed class Square : Shape
{
    public Square(int x, int y, Color color) : base(x, y, color) { }

    public override bool IsInside(int nx, int ny)
    {
        throw new NotImplementedException();
    }

    public override void Draw(DrawingContext context)
    {
        Brush lineBrush = new SolidColorBrush(color);
        Pen pen = new(lineBrush, 2, lineCap: PenLineCap.Square);
        var r2 = r / (float)Math.Sqrt(2);
        var point1 = new Point(x - r2, y + r2);
        var point2 = new Point(x + r2, y + r2);
        var point3 = new Point(x + r2, y - r2);
        var point4 = new Point(x - r2, y - r2);
        context.DrawLine(pen, point1, point2);
        context.DrawLine(pen, point2, point3);
        context.DrawLine(pen, point3, point4);
        context.DrawLine(pen, point4, point1);
    }
}