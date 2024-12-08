namespace Polygons.Models;

public abstract class Shape
{
    protected int x, y;
    static int r;

    protected Shape(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    static Shape()
    {
        r = 25;
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
}