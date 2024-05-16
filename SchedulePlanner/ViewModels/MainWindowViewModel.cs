using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using ReactiveUI;
using SchedulePlanner.Views;

namespace SchedulePlanner.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
#pragma warning disable CA1822 // Mark members as static
    private object? _currentView;
    public object? CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    private async Task Prepare()
    {
        var isAuth = false;

        CurrentView = isAuth ? new HomeView() : new AuthView();
    }
    
    public MainWindowViewModel()
    {
        //Preparing for further actions as well as checking authentication
        _ = Prepare();

    }
#pragma warning restore CA1822 // Mark members as static
}