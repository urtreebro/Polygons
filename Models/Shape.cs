using Avalonia.Media;

namespace Polygons.Models;

public abstract class Shape
{
    protected int x, y;
    protected static readonly int R;
    protected static Color color;

    public bool IsMoving { get; set; }

    public abstract bool IsInside(int nx, int ny);

    protected Shape(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    static Shape()
    {
        R = 35;
        color = Colors.Orange;
    }

    public int X
    {
        get => x;
        set => x = value;
    }

    public int Y
    {
        get => y;
        set => y = value;
    }

    public abstract void Draw(DrawingContext context);
}