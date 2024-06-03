using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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

public sealed class ClassesViewModel : ReactiveObject
{
    private readonly Group _group;
    public ICommand AddClassCommand { get; }
    public ICommand SelectDayOfWeekCommand { get; }
    public ICommand RemoveClassCommand { get; }
    public ICommand EditClassCommand { get; }
    public ICommand ResetFieldsCommand { get; }
    
    [Reactive] 
    public DayOfWeek SelectedDay { get; private set; }

    [Reactive] public string Title { get; private set; } = "Add new class";
    [Reactive] public string ButtonTitle { get; private set; } = "Add";
    
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

    [Reactive]
    public UniversityClass? EditingClass { get; set; }
    
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

    private void EditClass(UniversityClass uClass)
    {
        EditingClass = uClass;
        Title = "Editing " + uClass.SubjectName;
        SubjectName = uClass.SubjectName;
        TeacherName = uClass.TeacherName;
        ClassroomName = uClass.ClassroomName;
        StartsAt = uClass.StartsAt.ToTimeSpan();
        Duration = uClass.Duration;
        ButtonTitle = "Update";
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
        var overlappedClass = CurrentSchedule.FirstOrDefault(cls => cls != EditingClass &&
            startTime < cls.EndAt && endTime > cls.StartsAt);
        if (overlappedClass != null && overlappedClass != EditingClass)
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
            GroupId = _group.Id,
        };

        if (EditingClass != null)
        {
            entity.Id = EditingClass.Id;
            await App.Backend.AuthorizedRequest<Unit?>(new(Endpoints.ClassUpdate, HttpMethod.Patch)
            {
                Content =  WebService.ToJson(entity),
                OnSuccess = _ => Dispatcher.UIThread.Invoke(() =>
                {

                    CurrentSchedule.Replace(EditingClass,entity);
                    SetClasses(CurrentSchedule);
                    ResetFields();
                }),
                OnFailed = () => ErrorMsg = "An error has occurred"
            });
            return;
        }

        await App.Backend.AuthorizedRequest<UniversityClass>(new(Endpoints.ClassAdd, HttpMethod.Post)
        {
            Content =  WebService.ToJson(entity),
            OnSuccess = o => Dispatcher.UIThread.Invoke(() =>
            {
                CurrentSchedule.Add(o!);
                SetClasses(CurrentSchedule);
                ResetFields();
            }),
            OnFailed = () => ErrorMsg = "An error has occurred"
        });
    }

    private void ResetFields()
    {
        Title = "Add new class";
        EditingClass = null;
        ButtonTitle = "Add";
        ErrorMsg = SubjectName = TeacherName = ClassroomName = "";
        StartsAt = null;
        Duration = 90;
    }

    public ClassesViewModel(Group group)
    {
        _group = group;
        PartialView = !App.MainVm.IsAdmin;
        AddClassCommand = ReactiveCommand.CreateFromTask(AddClass);

        RemoveClassCommand = ReactiveCommand.CreateFromTask<UniversityClass>(RemoveClass);
        EditClassCommand = ReactiveCommand.Create<UniversityClass>(EditClass);
        ResetFieldsCommand = ReactiveCommand.Create(ResetFields);

        SelectDayOfWeekCommand = ReactiveCommand.CreateFromTask<DayOfWeek>(SelectDayOfWeek);
        _ = SelectDayOfWeek(DayOfWeek.Monday);
    }

    public ClassesViewModel() { }
}