using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SchedulePlanner.Models;
using SchedulePlanner.Tools.Backend;

namespace SchedulePlanner.ViewModels;

public sealed class StudentAddingViewModel : ReactiveObject
{
    private readonly Group _group;
    private string _searchbarText;
    private bool _waiting;
    [Reactive] public string? Status { get; set; } = "Enter 2 or more characters to search";
    [Reactive] public ObservableCollection<Student> Students { get; set; } = new();
    public ICommand ToggleGroupCommand { get; }
    
    private bool _typing;

    public string SearchbarText
    {
        get => _searchbarText;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchbarText,value);

            if (_searchbarText.Length < 2)
            {
                if (Students.Count != 0)
                {
                    Students = new();
                }
                
                Status = "Enter 2 or more characters to search";
                return;
            }
            _typing = true;
            if (!_waiting)
            {
                SearchDebounced();
            }
        }
    }
    
    private async void SearchDebounced()
    {
        _waiting = true;
        Status = "Searching...";
        while (_typing)
        {
            _typing = false;
            await Task.Delay(1000);
        }

        if (_searchbarText.Length < 2)
        {
            _waiting = false;
            return;
        }
        
        await App.Backend.AuthorizedRequest<ObservableCollection<Student>>
            (new(Endpoints.StudentSearch+$"email={_searchbarText}")
                { OnSuccess = o =>
                    {
                        foreach (var student in o!)
                        {
                            if (student.GroupId != null && student.GroupId.Value == _group.Id)
                            {
                                student.IsSameGroup = true;
                            }
                        }
                        Students = o;
                    }
                });
        Status = null;
        _waiting = false;
        if (Students.Count == 0)
        {
            Status = "Nothing found";
        }
    }

    private async Task OnGroupToggle(Student student)
    {
        var newEntity = new Student()
        {
            Email = student.Email,
            IsSameGroup = !student.IsSameGroup,
            UserId = student.UserId,
            GroupId = student.IsSameGroup ? null : _group.Id,
            GroupName = student.IsSameGroup ? null : _group.Name
        };
            
        await App.Backend.AuthorizedRequest<Unit>(new(Endpoints.StudentAddGroup + 
                                                      $"studentId={newEntity.UserId}&groupId={newEntity.GroupId}")
        {
            OnSuccess = _ => Dispatcher.UIThread.Invoke(() => Students.Replace(student,newEntity ))
        });
    }

    public StudentAddingViewModel(Group group)
    {
        _group = group;
        ToggleGroupCommand = ReactiveCommand.CreateFromTask<Student>(OnGroupToggle);
    }
}