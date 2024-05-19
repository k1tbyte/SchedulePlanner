using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SchedulePlanner.Controls;
using SchedulePlanner.Models;
using SchedulePlanner.Tools;
using SchedulePlanner.Tools.Backend;

namespace SchedulePlanner.ViewModels;

public class AdminViewModel : ReactiveObject
{
    public ReactiveCommand<Unit,Unit> AddEntityCommand { get; }
    
    [Reactive]
    public ObservableCollection<Department> Departments { get; private set; }

    private async Task FetchDepartments()
    {
        var result = await App.Backend.AuthorizedRequest<List<Department>>(
            new(Endpoints.AllDeps, HttpMethod.Get)
                { OnSuccess = o => Departments = new(o)});
    }

    private void OnEntityAdding()
    {
        ModalWindow.Open(new Form([
                    ("Name",new TextBox{ Watermark = "New department" }),
                ],
                async (reject, formData) =>
                {
                    var data = new Department { Name = formData[0]!.ToString() };
                    await App.Backend.AuthorizedRequest<Unit>(new(Endpoints.AddDept, HttpMethod.Post)
                    {
                        Content = WebService.ToJson(data),
                        OnSuccess = _ => Dispatcher.UIThread.Invoke(() => Departments.Add(data)),
                        OnFailed = () => reject("An error has occurred")
                    });
                }),
            "Adding a department"
        );
    }
    
    public AdminViewModel()
    {
        AddEntityCommand = ReactiveCommand.Create(OnEntityAdding);
        _ = FetchDepartments();
    }
}