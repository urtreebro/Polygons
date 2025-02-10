using Avalonia.Media;

namespace Polygons.Models;

public abstract class Shape
{
    protected double x, y;
    protected static readonly int R;
    protected static Color color;

    public bool IsInConvexHull { get; set; } = true;

    public bool IsMoving { get; set; }

    public abstract bool IsInside(double nx, double ny);

    protected Shape(double x = 0, double y = 0)
    {
        this.x = x;
        this.y = y;
    }

    static Shape()
    {
        R = 35;
        color = Colors.Orange;
    }

    public double X
    {
        get => x;
        set => x = value;
    }

    public double Y
    {
        get => y;
        set => y = value;
    }

    public abstract void Draw(DrawingContext context);
}