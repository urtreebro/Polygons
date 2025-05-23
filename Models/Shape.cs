﻿using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Avalonia.Media;

namespace Polygons.Models;

[JsonConverter(typeof(ShapeConverter))]
public abstract class Shape
{
    protected double x, y;
    public static double R;
    public static Color color;

    public bool IsInConvexHull { get; set; } = true;

    public string Type { get; set; }
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

    public double InitialX { get; set; }
    
    public double InitialY { get; set; }

    public abstract void Draw(DrawingContext context);
}