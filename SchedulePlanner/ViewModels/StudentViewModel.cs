using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SchedulePlanner.Tools.Backend;
using SchedulePlanner.Views;
using Group = SchedulePlanner.Models.Group;

namespace SchedulePlanner.ViewModels;

public class StudentViewModel : ReactiveObject
{
    [Reactive] public object Content { get; set; }

    [Reactive] public string Status { get; set; } = "Loading";
    private Group? _group;

    private async Task FetchGroup()
    {
        await App.Backend.Request<Group?>(new(
            Endpoints.Group +
            $"/getbyusername?username={App.Backend.CurrentSession?.JwtAccess?.Claims.First(o => o.Type == "username").Value}")
        {
            OnSuccess = o => _group = o,
            OnFailed = () => Status = "Fetching error"
        });
        
        if (_group == null)
        {
            Status = "You are not a member of any group";
            return;
        }

        Content = Dispatcher.UIThread.Invoke(() =>  new ClassesView(_group));
        Status = $"Weekly class schedule for your group: {_group.Name} ({_group.Year} course)";
    }

    public StudentViewModel()
    {
        Task.Factory.StartNew(FetchGroup);
    }
}