using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Polygons.Views;

public partial class ParallelTestWindow : Window
{
    public ParallelTestWindow()
    {
        InitializeComponent();
        ParallelTestControl? testControl = this.Find<ParallelTestControl>("MyParallelTestControl");
        testControl?.Test();
    }
}