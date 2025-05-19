using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Fonts;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Polygons.Models;
using Color = Avalonia.Media.Color;

namespace Polygons.Views;

public partial class MainWindow : Window
{
    private SliderWindow? _sliderWindow;
    private string? _filename;
    private readonly Timer _timer;

    public MainWindow()
    {
        InitializeComponent();
        Shapes.ItemsSource = new[] { "Circle", "Triangle", "Square" };
        Shapes.SelectedIndex = 1;
        Algorithms.ItemsSource = new[] { "By definition", "Jarvis" };
        Algorithms.SelectedIndex = 0;
        _timer = new Timer(200);
        _timer.Elapsed += TimerTick;
        _timer.Enabled = false;
        _timer.AutoReset = true;
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        Action.MyCustomControl = customControl!;
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        var point = e.GetCurrentPoint(sender as CustomControl);
        if (point.Properties.IsLeftButtonPressed)
        {
            customControl?.LeftClick((int)e.GetPosition(customControl).X, (int)e.GetPosition(customControl).Y);
        }

        if (point.Properties.IsRightButtonPressed)
        {
            customControl?.RightClick((int)e.GetPosition(customControl).X, (int)e.GetPosition(customControl).Y);
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        customControl?.Move((int)e.GetPosition(customControl).X, (int)e.GetPosition(customControl).Y);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        customControl?.Release((int)e.GetPosition(customControl).X, (int)e.GetPosition(customControl).Y);
    }

    private void Shapes_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");

        int type = Shapes.SelectedIndex;
        customControl?.ChangeType(type);
    }

    private void Button_OnClickParallel(object? sender, RoutedEventArgs e)
    {
        var window = new ParallelTestWindow();
        window.Show();
    }

    private void Button_OnClickSerial(object? sender, RoutedEventArgs e)
    {
        var window = new SerialTestWindow();
        window.Show();
    }

    private void Algorithms_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");

        int type = Algorithms.SelectedIndex;
        customControl?.ChangeAlgorithm(type);
    }

    private void Button_OnClickCheckPerformance(object? sender, RoutedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");

        var window = new ChartWindow(customControl?.GetChartJarvis(), customControl?.GetChartByDef(), 3);
        window.Show();
    }

    private void Button_OnClickCheckByDef(object? sender, RoutedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");

        var window = new ChartWindow(null, customControl?.GetChartByDef(), 1);
        window.Show();
    }

    private void Button_OnClickCheckJarvis(object? sender, RoutedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");

        var window = new ChartWindow(customControl?.GetChartJarvis(), null, 2);
        window.Show();
    }

    private void Button_OnClickChangeRadius(object? sender, RoutedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        if (_sliderWindow is not { IsLoaded: true })
        {
            _sliderWindow = new SliderWindow();
            _sliderWindow.SetRadius(Shape.R);
            _sliderWindow.RadiusChanged += customControl!.UpdateRadius;
            _sliderWindow.Show();
        }
        else
        {
            _sliderWindow.Activate();
            _sliderWindow.WindowState = WindowState.Normal;
        }
    }

    private async Task<string> SaveAs(CustomControl customControl)
    {
        var topLevel = GetTopLevel(this);

        var file = await topLevel!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Text File",
            FileTypeChoices = [Json],
        });
        if (file is null) return "cancelled";
        _filename = file.Name;
        FileSaver.SaveFile(customControl, file.Name);
        customControl.IsChanged = false;
        return "saved";
    }

    private async Task<string> Save(CustomControl customControl)
    {
        if (_filename == null)
        {
            var res = await SaveAs(customControl);
            return res;
        }

        FileSaver.SaveFile(customControl, _filename);
        customControl.IsChanged = false;
        return "saved";
    }

    private async void Button_Save(object? sender, RoutedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        await Save(customControl!);
    }

    private async void Button_SaveAs(object? sender, RoutedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        await SaveAs(customControl!);
    }

    private async void Button_New(object? sender, RoutedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        if (!CheckIfFileSaved())
        {
            var res = await SaveDialog();
            switch (res)
            {
                case "yes":
                    var answer = await Save(customControl!);
                    if (answer == "cancelled")
                    {
                        return;
                    }

                    break;
                case "no":
                    break;
                default:
                    return;
            }
        }

        CloseOther();
        customControl!.Shapes = [];
        Shape.R = 35;
        Shape.color = Colors.Orange;
        _filename = null;
    }

    private async void Button_Open(object? sender, RoutedEventArgs e)
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        if (!CheckIfFileSaved())
        {
            var res = await SaveDialog();
            switch (res)
            {
                case "yes":
                    var answer = await Save(customControl!);
                    if (answer == "cancelled")
                    {
                        return;
                    }

                    break;
                case "no":
                    break;
                default:
                    return;
            }
        }

        var topLevel = GetTopLevel(this);

        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open File",
            AllowMultiple = false,
            FileTypeFilter = [Json]
        });

        if (files.Count == 0) return;
        await using var stream = await files[0].OpenReadAsync();
        using var streamReader = new StreamReader(stream);
        var fileContent = await streamReader.ReadToEndAsync();
        CloseOther();
        FileSaver.OpenFile(customControl!, fileContent);
        _filename = files[0].Name;
        customControl!.IsChanged = false;
    }

    private void Button_Exit(object? sender, RoutedEventArgs e)
    {
        Close();
    }


    private bool CheckIfFileSaved()
    {
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        return customControl!.Shapes.Count == 0 || !customControl.IsChanged;
    }

    private async Task<string> SaveDialog()
    {
        var dialog = new SaveDialogWindow();

        var res = await dialog.ShowDialog<string>(this);
        Console.WriteLine(res);
        switch (res)
        {
            case "Yes":
                return "yes";
            case "Cancel":
                return "cancel";
            case "No":
                return "no";
            default:
                return "error";
        }
    }

    private async void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (!CheckIfFileSaved())
        {
            e.Cancel = true;

            CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
            if (!CheckIfFileSaved())
            {
                var res = await SaveDialog();
                switch (res)
                {
                    case "yes":
                        var answer = await Save(customControl!);
                        if (answer == "saved")
                        {
                            Closing -= Window_OnClosing;
                            Close();
                        }

                        return;
                    case "cancel":
                        return;
                }
            }
        }

        Closing -= Window_OnClosing;
        Close();
    }

    private void CloseOther()
    {
        _sliderWindow?.Close();
    }

    private static FilePickerFileType Json { get; } = new("JSON")
    {
        Patterns = ["*.json"],
        AppleUniformTypeIdentifiers = ["public.json"],
        MimeTypes = ["json/*"],
    };

    private async void Button_OnClickChangeColor(object? sender, RoutedEventArgs e)
    {
        var colorWindow = new ColorWindow();
        colorWindow.setColor(Shape.color);
        var color = await colorWindow.ShowDialog<Color>(this);
        if (color == Color.FromUInt32(0)) return;
        CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
        customControl!.UpdateColor(color);
    }

    private void TimerTick(object? sender, ElapsedEventArgs e)
    {
        if (!_timer.Enabled) return;
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            CustomControl? customControl = this.Find<CustomControl>("MyCustomControl");
            customControl!.MoveShapes();
            return Task.CompletedTask;
        });
    }

    private void TimerEnabled_OnClick(object? sender, RoutedEventArgs e)
    {
        _timer.Enabled = !_timer.Enabled;
    }

    private void UndoButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Action.UndoAction();
    }

    private void RedoButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Action.RedoAction();
    }
}