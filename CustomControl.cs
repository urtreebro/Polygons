﻿using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Polygons.Models;

namespace Polygons;

public class CustomControl : UserControl
{
    private readonly List<Shape> _shapes = [];

    private int _prevX, _prevY;
    private int _shapeType;

    public override void Render(DrawingContext context)
    {
        foreach (var shape in _shapes)
        {
            shape.Draw(context);
        }

        Console.WriteLine("Drawing");
    }

    public void LeftClick(int newX, int newY)
    {
        bool inside = false;
        foreach (var shape in _shapes.Where(shape => shape.IsInside(newX, newY)))
        {
            Console.WriteLine("Click");
            _prevX = newX;
            _prevY = newY;
            shape.IsMoving = true;
            inside = true;
        }

        if (!inside)
        {
            switch (_shapeType)
            {
                case 0:
                    _shapes.Add(new Circle(newX, newY));
                    break;
                case 1:
                    _shapes.Add(new Triangle(newX, newY));
                    break;
                case 2:
                    _shapes.Add(new Square(newX, newY));
                    break;
            }
        }

        InvalidateVisual();
    }

    public void RightClick(int newX, int newY)
    {
        foreach (var shape in _shapes.Where(shape => shape.IsInside(newX, newY)).Reverse())
        {
            _prevX = newX;
            _prevY = newY;
            _shapes.Remove(shape);
            break;
        }

        InvalidateVisual();
    }

    public void Move(int newX, int newY)
    {
        foreach (var shape in _shapes.Where(shape => shape.IsMoving))
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
        foreach (var shape in _shapes.Where(shape => shape.IsMoving))
        {
            Console.WriteLine("Release");
            shape.X += newX - _prevX;
            shape.Y += newY - _prevY;
            shape.IsMoving = false;
        }

        _prevX = newX;
        _prevY = newY;
        InvalidateVisual();
    }

    public void ChangeType(int type)
    {
        _shapeType = type;
    }
}