using Avalonia.Media;

namespace Polygons.Models;

public abstract class Shape
{
    protected int x, y;
    protected static readonly int R;
    protected Color color;

    public bool IsMoving { get; set; }

    public abstract bool IsInside(int nx, int ny);

    protected Shape(int x, int y, Color color)
    {
        this.x = x;
        this.y = y;
        this.color = color;
    }

    static Shape()
    {
        R = 35;
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