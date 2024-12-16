using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Polygons.Models;

namespace Polygons;

public class CustomControl : UserControl
{
    private readonly List<Shape> _shapes =
    [
        new Circle(100, 100, Colors.Orange),
        new Square(100, 100, Colors.Magenta),
        new Triangle(300, 300, Colors.Cyan),
    ];

    private readonly List<Shape> _draggedShapes = [];
    private int _prevX, _prevY;

    public override void Render(DrawingContext context)
    {
        foreach (var shape in _shapes)
        {
            shape.Draw(context);
        }

        Console.WriteLine("Drawing");
    }

    public void Click(int newX, int newY)
    {
        foreach (var shape in _shapes.Where(shape => shape.IsInside(newX, newY)))
        {
            Console.WriteLine("Click");
            _prevX = newX;
            _prevY = newY;
            shape.IsMoving = true;
            _draggedShapes.Add(shape);
        }

        InvalidateVisual();
    }

    public void Move(int newX, int newY)
    {
        foreach (var shape in _shapes.Where(shape => _draggedShapes.Contains(shape)))
        {
            Console.WriteLine("Move");
            shape.X += newX - _prevX;
            shape.Y += newY - _prevY;
        }

        _prevX = newX;
        _prevY = newY;
        InvalidateVisual();
    }

    public void Release(int newX, int newY)
    {
        foreach (var shape in _shapes.Where(shape => _draggedShapes.Contains(shape)))
        {
            Console.WriteLine("Release");
            shape.X += newX - _prevX;
            shape.Y += newY - _prevY;
            shape.IsMoving = false;
            _draggedShapes.Remove(shape);
        }

        _prevX = newX;
        _prevY = newY;
        InvalidateVisual();
    }
}