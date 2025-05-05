using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Polygons.Models;

namespace Polygons.Views;

public partial class ColorWindow : Window
{
    public ColorWindow()
    {
        InitializeComponent();
    }

    public void setColor(Color color)
    {
        Picker.Color = color;
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var color = Picker.Color;
        Close(color);
    }
    
}