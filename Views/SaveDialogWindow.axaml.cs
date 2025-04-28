using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Polygons.Views;

public partial class SaveDialogWindow : Window
{
    public SaveDialogWindow()
    {
        InitializeComponent();
    }

    private void Button_Yes(object? sender, RoutedEventArgs e)
    {
        Close("Yes");
    }

    private void Button_No(object? sender, RoutedEventArgs e)
    {
        Close("No");
    }

    private void Button_Cancel(object? sender, RoutedEventArgs e)
    {
        Close("Cancel");
    }
}