using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
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
        RadiusSlider.Value = r;
    }

    public event RadiusDelegate RadiusChanged;


    private void RadiusSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (RadiusChanged != null)
        {
            RadiusChanged(this, new RadiusEventArgs(RadiusSlider.Value));
        }
    }
}