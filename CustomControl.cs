using System;
using Avalonia.Controls;
using Avalonia.Media;
using Polygons.Models;

namespace Polygons;

public partial class CustomControl : UserControl
{
    private readonly Shape shape = new Circle(500, 500, Colors.Blue);
    private int px, py;
    public override void Render(DrawingContext context)
    {
        shape.Draw(context);

        Console.WriteLine("Drawing");
    }

    public void Click(int nx, int ny)
    {
        if (!shape.IsInside(nx, ny)) return;
        Console.WriteLine("Click");
        px = nx;
        py = ny;
        shape.IsMoving = true;
        InvalidateVisual();
    }

    public void Move(int nx, int ny)
    {
        if (!shape.IsMoving) return;
        Console.WriteLine("Move");
        shape.X += nx - px;
        shape.Y += ny - py;
        px = nx;
        py = ny;
        InvalidateVisual();
    }

    public void Release(int nx, int ny)
    {
        if (!shape.IsMoving) return;
        Console.WriteLine("Release");
        shape.X += nx - px;
        shape.Y += ny - py;
        px = nx;
        py = ny;
        shape.IsMoving = false;
        InvalidateVisual();
    }
}