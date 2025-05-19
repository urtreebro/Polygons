using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Polygons.Models;

namespace Polygons;

public class RadiusSlider : RangeBase
{
    private double _startValue;

    static RadiusSlider()
    {
        Thumb.DragStartedEvent.AddClassHandler<RadiusSlider>((x, e) => x.OnThumbDragStarted(e));
        Thumb.DragDeltaEvent.AddClassHandler<RadiusSlider>((x, e) => x.OnThumbDragDelta(e));
        Thumb.DragCompletedEvent.AddClassHandler<RadiusSlider>((x, e) => x.OnThumbDragCompleted(e));
    }

    public Track? Track { get; private set; }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        Track = e.NameScope.Find<Track>("PART_Track");
    }

    private void OnThumbDragStarted(VectorEventArgs e)
    {
        _startValue = Value;
    }

    private void OnThumbDragDelta(VectorEventArgs e)
    {
        if (Track?.Thumb == null) return;

        var newValue = Value + e.Vector.X * (Maximum - Minimum) / (Track.Bounds.Width - Track.Thumb.Bounds.Width);
        Value = Math.Max(Minimum, Math.Min(Maximum, newValue));
    }

    private void OnThumbDragCompleted(VectorEventArgs e)
    {
        Action.Add(new ChangeRadius(_startValue, Value));
    }
}