using System.Collections.Generic;
using Avalonia.Media;
using Polygons.Models;

namespace Polygons;

public abstract class Action
{
    private static readonly Stack<Action> History;
    private static readonly Stack<Action> ToRedo;
    public static CustomControl MyCustomControl = null!;

    static Action()
    {
        History = new Stack<Action>();
        ToRedo = new Stack<Action>();
    }

    public abstract void Undo();
    public abstract void Redo();

    public static void Add(Action action)
    {
        if (ToRedo.Count != 0) ToRedo.Clear();
        History.Push(action);
    }
    
    public static void UndoAction()
    {
        if (History.Count == 0) return;
        History.Peek().Undo();
        ToRedo.Push(History.Peek());
        History.Pop();
        MyCustomControl.InvalidateVisual();
    }

    public static void RedoAction()
    {
        if (ToRedo.Count == 0) return;
        ToRedo.Peek().Redo();
        History.Push(ToRedo.Peek());
        ToRedo.Pop();
        MyCustomControl.InvalidateVisual();
    }
}

public class Combination : Action
{
    private readonly List<Action> _actions;

    public Combination(List<Action> actions)
    {
        _actions = actions;
    }

    public override void Undo()
    {
        _actions.Reverse();
        foreach (var action in _actions)
        {
            action.Undo();
        }
    }

    public override void Redo()
    {
        _actions.Reverse();
        foreach (var action in _actions)
        {
            action.Redo();
        }
    }
}

public class AddShape : Action
{
    private readonly Shape _shape;

    public AddShape(Shape shape)
    {
        _shape = shape;
    }

    public override void Undo()
    {
        MyCustomControl.Shapes.Remove(_shape);
    }

    public override void Redo()
    {
        MyCustomControl.Shapes.Add(_shape);
    }
}

public class RemoveShape : Action
{
    private readonly Shape _shape;

    public RemoveShape(Shape shape)
    {
        _shape = shape;
    }
    
    public override void Undo()
    {
        MyCustomControl.Shapes.Add(_shape);
    }

    public override void Redo()
    {
        MyCustomControl.Shapes.Remove(_shape);
    }
}

public class MoveShape : Action
{
    private readonly double _prevX;
    private readonly double _prevY;
    private readonly double _currX;
    private readonly double _currY;
    
    private readonly Shape _shape;

    public MoveShape(Shape shape, double prevX, double prevY, double currX, double currY)
    {
        _prevX = prevX;
        _prevY = prevY;
        _currY = currY;
        _currX = currX;
        _shape = shape;
    }
    public override void Undo()
    {
        MyCustomControl.Shapes.Find(x => x == _shape)!.X = _prevX;
        MyCustomControl.Shapes.Find(x => x == _shape)!.Y = _prevY;
    }

    public override void Redo()
    {
        MyCustomControl.Shapes.Find(x => x == _shape)!.X = _currX;
        MyCustomControl.Shapes.Find(x => x == _shape)!.Y = _currY;
    }
}

public class ChangeRadius : Action
{
    private readonly double _oldR;
    private readonly double _newR;

    public ChangeRadius(double oldR, double newR)
    {
        _oldR = oldR;
        _newR = newR;
    }

    public override void Undo()
    {
        Shape.R = _oldR;
    }

    public override void Redo()
    {
        Shape.R = _newR;
    }
}

public class ChangeColor : Action
{
    private readonly Color _oldColor;
    private readonly Color _newColor;

    public ChangeColor(Color oldColor, Color newColor)
    {
        _oldColor = oldColor;
        _newColor = newColor;
    }

    public override void Undo()
    {
        Shape.color = _oldColor;
    }

    public override void Redo()
    {
        Shape.color = _newColor;
    }
}
