using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Polygons.Models;

namespace Polygons;

public partial class CustomControl : UserControl
{
    public override void Render(DrawingContext context)
    {
        List<Shape> shapes =
        [
            new Circle(100, 100, Colors.Red),
            new Circle(300, 300, Colors.Blue),
            new Triangle(200, 400, Colors.Orange),
            new Square(500, 300, Colors.Green),
            new Square(600, 400, Colors.Cyan),
        ];
        foreach (var s in shapes)
        {
            s.Draw(context);
        }

        Console.WriteLine("Drawing");
    }
}