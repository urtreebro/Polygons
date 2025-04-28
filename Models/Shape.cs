using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Avalonia.Media;

namespace Polygons.Models;

[JsonConverter(typeof(ShapeConverter))]
public abstract class Shape
{
    protected double x, y;
    public static double R;
    protected static Color color;

    public bool IsInConvexHull { get; set; } = true;

    public string Type { get; set; }
    public bool IsMoving { get; set; }

    public abstract bool IsInside(double nx, double ny);

    public Shape() {}
    
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

    // [JsonConstructor]
    // protected Shape(bool IsInConvexHull, bool IsMoving, double X, double Y)
    // {
    //     this.IsInConvexHull = IsInConvexHull;
    //     this.IsMoving = IsMoving;
    //     this.X = X;
    //     this.Y = Y;
    // }

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