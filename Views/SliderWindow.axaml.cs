using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;

namespace Polygons.Views;

public partial class SliderWindow : Window
{
    public SliderWindow()
    {
        InitializeComponent();
    }

    public void SetRadius(double r)
    {
        Slider.Value = r;
    }

    public event RadiusDelegate RadiusChanged;

    private void RangeBase_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (RadiusChanged != null)
        {
            RadiusChanged(this, new RadiusEventArgs(Slider.Value));
        }
    }
}