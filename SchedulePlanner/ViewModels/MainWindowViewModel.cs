using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SchedulePlanner.Controls;
using SchedulePlanner.Views;

namespace SchedulePlanner.ViewModels;


public class MainWindowViewModel : ReactiveObject
{
    [Reactive]
    public object? CurrentView { get; private set; }

    private async Task Prepare()
    {
        var isAuth = await App.Backend.Authorize();
        CurrentView = isAuth.Success ? new HomeView() : new AuthView();
    }

    public void SetView(object view) => 
        Dispatcher.UIThread.Invoke(() => CurrentView = view);

    public MainWindowViewModel()
    {
        //Preparing for further actions as well as checking authentication
        if (Design.IsDesignMode)
        {
            return;
        }

        App.Backend.OnUnauthorized += () =>
        {
            SetView(new AuthView());
            ModalWindow.Open("Your login session has expired, please log in to your account again",
                "Re-login required");
        };
 
        _ = Prepare();

    }
}