using System;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SchedulePlanner.Tools;
using SchedulePlanner.ViewModels;
using SchedulePlanner.Views;

namespace SchedulePlanner;

public partial class App : Application
{
    public static MainWindow MainWindow { get; private set; } = null!;
    public static MainWindowViewModel MainVm => (MainWindow.DataContext as MainWindowViewModel)!;
    public static readonly WebService Backend = new();
    public static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    public override void Initialize()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Backend.Dispose();
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}