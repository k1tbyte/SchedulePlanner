using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SchedulePlanner.Models;
using SchedulePlanner.Tools.Backend;

namespace SchedulePlanner.ViewModels;

public sealed class ClassesViewModel : ReactiveObject
{
    private readonly Group _group;
    public ICommand AddClassCommand { get; }
    public ICommand SelectDayOfWeekCommand { get; }
    public ICommand RemoveClassCommand { get; }
    
    [Reactive] 
    public DayOfWeek SelectedDay { get; private set; }
    
    public IEnumerable<DayOfWeek> DaysOfWeek { get; private set; } =
        (Enum.GetValues(typeof(DayOfWeek)) as IEnumerable<DayOfWeek>).Skip(1)!;
    
    [Reactive] public string SubjectName { get; set; }
    [Reactive] public string TeacherName { get; set; }
    [Reactive] public string ClassroomName { get; set; }
    [Reactive] public TimeSpan? StartsAt { get; set; }
    [Reactive] public int Duration { get; set; } = 90;
    [Reactive] public string ErrorMsg { get; set; }
    public bool PartialView { get; }
    

    [Reactive]
    public ObservableCollection<UniversityClass> CurrentSchedule { get; set; } = new();
    
    public static bool IsEmptyString(params string[] strings) => strings.Any(string.IsNullOrEmpty);

    private async Task SelectDayOfWeek(DayOfWeek day)
    {
        if (day == SelectedDay)
            return;

        await App.Backend.Request<UniversityClass[]>(
            new(Endpoints.ClassGetByDay + $"groupId={_group.Id}&dayOfWeek={(int)day}")
        {
            OnSuccess = o =>
            {
                SetClasses(o!);
                SelectedDay = day;
            }
        });
    }
    
    private async Task RemoveClass(UniversityClass universityClass)
    {
        await App.Backend.Request<Unit>(new(Endpoints.Class + $"/delete?id={universityClass.Id}", HttpMethod.Delete)
        {
            OnSuccess = _ => CurrentSchedule.Remove(universityClass)
        });
    }

    private void SetClasses(IEnumerable<UniversityClass> classList)
    {
        CurrentSchedule = new(classList.OrderBy(o => o.StartsAt));
    }

    private async Task AddClass()
    {
        if (IsEmptyString(SubjectName, TeacherName, ClassroomName) || StartsAt == null)
        {
            ErrorMsg = "Fill in all required fields";
            return;
        }

        if (Duration < 20)
        {
            ErrorMsg = "Invalid duration, minimum 20";
            return;
        }

        if (StartsAt < TimeSpan.FromHours(7) || StartsAt + TimeSpan.FromMinutes(Duration) > TimeSpan.FromHours(18))
        {
            ErrorMsg = "Acceptable hours for classes: 7:00 - 18:00";
            return;
        }

        var startTime = TimeOnly.FromTimeSpan(StartsAt.Value);
        var endTime = startTime.AddMinutes(Duration);
        var overlappedClass = CurrentSchedule.FirstOrDefault(cls =>
            startTime < cls.EndAt && endTime > cls.StartsAt);
        if (overlappedClass != null)
        {
            ErrorMsg = "The class overlaps with " + overlappedClass.SubjectName;
            return;
        }

        var entity = new UniversityClass()
        {
            ClassroomName = ClassroomName,
            Duration = Duration,
            TeacherName = TeacherName,
            StartsAt = startTime,
            SubjectName = SubjectName,
            DayOfWeek = SelectedDay,
            GroupId = _group.Id
        };

        await App.Backend.AuthorizedRequest<UniversityClass>(new(Endpoints.ClassAdd, HttpMethod.Post)
        {
            Content =  WebService.ToJson(entity),
            OnSuccess = o => Dispatcher.UIThread.Invoke(() =>
            {
                CurrentSchedule.Add(o!);
                SetClasses(CurrentSchedule);
                ErrorMsg = SubjectName = TeacherName = ClassroomName = "";
                StartsAt = null;
                Duration = 90;
            }),
            OnFailed = () => ErrorMsg = "An error has occurred"
        });
    }

    public ClassesViewModel(Group group)
    {
        _group = group;
        PartialView = !App.MainVm.IsAdmin;
        AddClassCommand = ReactiveCommand.CreateFromTask(AddClass);

        RemoveClassCommand = ReactiveCommand.CreateFromTask<UniversityClass>(RemoveClass);

        SelectDayOfWeekCommand = ReactiveCommand.CreateFromTask<DayOfWeek>(SelectDayOfWeek);
        _ = SelectDayOfWeek(DayOfWeek.Monday);
    }

    public ClassesViewModel() { }
}