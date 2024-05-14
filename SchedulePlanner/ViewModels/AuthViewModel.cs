using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace SchedulePlanner.ViewModels;

public class AuthViewModel : ReactiveObject
{
    public ReactiveCommand<Unit,Unit> LoginCommand { get; } = ReactiveCommand.Create(() =>
    {
        Console.WriteLine("On login");
    });
    
    public ReactiveCommand<Unit,Unit> RegisterCommand { get; } = ReactiveCommand.CreateFromTask(async () =>
    {
        await Task.Delay(3000);
        Console.WriteLine("On register");
    });
}