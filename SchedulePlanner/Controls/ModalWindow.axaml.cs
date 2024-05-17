using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace SchedulePlanner.Controls;

public partial class ModalWindow : Grid
{
    private static ModalWindow Instance => App.MainWindow.Modal;

    public static void Open(object content, string title)
    {
        Instance.Presenter.Content = content;
        Instance.Title.Text = title;
        Instance.IsVisible = true;
        Instance.Splash.Opacity = 0.6;
    }
    
    public static void Close()
    {
        Instance.Presenter.Content = null;
        Instance.IsVisible = false;
        Instance.Splash.Opacity = 0;
    }

    public ModalWindow()
    {
        InitializeComponent();
    }

    private void OnBorderPressed(object? sender, PointerPressedEventArgs e) => Close();

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();
}