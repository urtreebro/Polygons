using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Polygons.Views;

public partial class SerialTestWindow : Window
{
    public SerialTestWindow()
    {
        InitializeComponent();
        SerialTestControl? testControl = this.Find<SerialTestControl>("MySerialTestControl");
        testControl?.Test();
    }
}