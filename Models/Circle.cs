using Avalonia;
using Avalonia.Media;

namespace Polygons.Models;

public sealed class Circle : Shape
{
    public Circle(double x, double y) : base(x, y) { }

    public override bool IsInside(double nx, double ny)
    {
        return (x - nx) * (x - nx) + (y - ny) * (y - ny) <= R * R;
    }

    public override void Draw(DrawingContext context)
    {
        Brush brush = new SolidColorBrush();
        Brush lineBrush = new SolidColorBrush(color);
        Pen pen = new(lineBrush, 2, lineCap: PenLineCap.Square);
        context.DrawEllipse(brush, pen, new Point(x, y), R, R);
    }
}