using System;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SchedulePlanner.Tools;
using SchedulePlanner.Views;

namespace SchedulePlanner.ViewModels;

public class HomeViewModel : ReactiveObject
{
    public bool IsAdmin { get; set; }
    
    [Reactive]
    public string Title { get; set; }
    
    [Reactive]
    public object? CurrentPage { get; private set; }
    public ReactiveCommand<Unit,Unit> SignOutCommand { get; }
    public ReactiveCommand<Unit,Unit> AdminPanelCommand { get; }
    public ReactiveCommand<Unit,Unit> ClassesCommand { get; }
    public ReactiveCommand<Unit,Unit> DashboardCommand { get; }

    private readonly (Lazy<AdminView>,string) _adminPage = (new(), "Admin panel");
    private readonly (Lazy<ClassesView>,string) _classesPage = (new(), "List of classes");
    private readonly (DashboardView,string) _dashboardPage  = (new(), "Dashboard");

    public void SetPage(Control page, string title)
    {
        CurrentPage = page;
        Title = title;
    }
    
    public HomeViewModel()
    {
        if (Design.IsDesignMode || App.Backend.CurrentSession?.JwtAccess?.Claims.First(o => o.Type == "accessRights").Value == "1")
        {
            IsAdmin = true;
        }
        
        SetPage(_dashboardPage.Item1, _dashboardPage.Item2);
        
        AdminPanelCommand = ReactiveCommand.Create(() => SetPage(_adminPage.Item1.Value, _adminPage.Item2));
        ClassesCommand = ReactiveCommand.Create(() => SetPage(_classesPage.Item1.Value, _classesPage.Item2));
        DashboardCommand = ReactiveCommand.Create(() => SetPage(_dashboardPage.Item1, _dashboardPage.Item2));
        SignOutCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await App.Backend.SignOut();
            App.MainVm.SetView(new AuthView());
        });
    }
}