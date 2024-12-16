using System;
using Avalonia;
using Avalonia.Media;

namespace Polygons.Models;

public sealed class Square : Shape
{
    private Point _point1, _point2, _point3, _point4;
    private static float InnerR => R / (float)Math.Sqrt(2);
    public Square(int x, int y, Color color) : base(x, y, color) { }

    public override bool IsInside(int newX, int newY)
    {
        return x - InnerR <= newX && newX <= x + InnerR && y - InnerR <= newY && newY <= y + InnerR;
    }

    public override void Draw(DrawingContext context)
    {
        Brush lineBrush = new SolidColorBrush(color);
        Pen pen = new(lineBrush, 2, lineCap: PenLineCap.Square);

        _point1 = new Point(x - InnerR, y + InnerR);
        _point2 = new Point(x + InnerR, y + InnerR);
        _point3 = new Point(x + InnerR, y - InnerR);
        _point4 = new Point(x - InnerR, y - InnerR);
        context.DrawLine(pen, _point1, _point2);
        context.DrawLine(pen, _point2, _point3);
        context.DrawLine(pen, _point3, _point4);
        context.DrawLine(pen, _point4, _point1);
    }
}