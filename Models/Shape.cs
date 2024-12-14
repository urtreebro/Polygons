using Avalonia.Media;

namespace Polygons.Models;

public abstract class Shape
{
    protected int x, y;
    protected static int r;
    protected Color color;

    protected Shape(int x, int y, Color color)
    {
        this.x = x;
        this.y = y;
        this.color = color;
    }

    static Shape()
    {
        r = 30;
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