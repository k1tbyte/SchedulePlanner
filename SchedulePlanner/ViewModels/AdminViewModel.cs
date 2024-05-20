using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SchedulePlanner.Controls;
using SchedulePlanner.Models;
using SchedulePlanner.Tools;
using SchedulePlanner.Tools.Backend;

namespace SchedulePlanner.ViewModels;

public enum EEntitySection
{
    None,
    Department,
    Speciality,
    Group
}

public class AdminViewModel : ReactiveObject
{
    public ICommand AddEntityCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand EntitySelectCommand { get; }
    public ICommand BreadcrumbCommand { get; }
    
    [Reactive]
    public ObservableCollection<INamedEntity>? CurrentCollection { get; private set; }
    [Reactive] public ObservableCollection<string> Breadcrumb { get; private set; } = new();
    
    private readonly Dictionary<EEntitySection,(ObservableCollection<INamedEntity> collection, INamedEntity? owner)> _cache = new();

    private string _entityName;

    private INamedEntity? _selectedDept;
    private INamedEntity? _selectedSpeciality;

    public byte sectionId;
    

    private async Task<(ObservableCollection<INamedEntity> collection, INamedEntity? owner)> GetCached<T>(
        string endpoint ,EEntitySection section, INamedEntity? entity = null
    ) where T : INamedEntity
    {
        
        if (_cache.TryGetValue(section,out var cached) && (entity == cached.owner || entity == null))
        {
            return cached;
        }

        var collection = new ObservableCollection<INamedEntity>(
            await FetchEntities<T>(endpoint) as INamedEntity[] ?? throw new Exception());
        _cache[section] = (collection, entity);
        return _cache[section];

    }

    // If entity is passed then we open a new tab - if null we go back
    private async Task SetSection(EEntitySection section, INamedEntity? entity)
    {
        Breadcrumb.Clear();
        switch (section)
        {
            case EEntitySection.None:
                Breadcrumb.Add("Departments");
                CurrentCollection = (await GetCached<Department>(Endpoints.DepartmentAll, section)).collection;
                _entityName = Endpoints.Department;
                break;
            
            case EEntitySection.Department: 
                var items= await GetCached<Speciality>(
                    Endpoints.SpecialityAll + entity?.Id, section, entity);
                _selectedDept = items.owner;
                Breadcrumb.AddRange(["Department: " + _selectedDept!.Name, "Specialities"]);
                CurrentCollection = items.collection;
                _entityName = Endpoints.Speciality;
                break;
            
            case EEntitySection.Speciality:
                var groups= await GetCached<Group>(
                    Endpoints.GroupAll + entity?.Id, section, entity);
                Breadcrumb.AddRange([
                    "Department: " + _selectedDept!.Name, "Speciality: " + groups.owner!.Name, "Groups"
                ]);
                _selectedSpeciality = groups.owner;
                CurrentCollection = groups.collection;
                _entityName = Endpoints.Group;
                break;
            case EEntitySection.Group:
                break;
        }

        sectionId = (byte)section;
    }

    private async Task<T[]?> FetchEntities<T>(string url)
    {
        T[]? collection = null;
        await App.Backend.AuthorizedRequest<T[]>(
            new(url, HttpMethod.Get) { OnSuccess = o => collection = o });
        return collection;

    }

    private void AddOrUpdate<T>(
        string title, (string, Control)[] fields, Func<object?[],INamedEntity?> callback, INamedEntity? original = null) 
        where T : INamedEntity
    {
        ModalWindow.Open(new Form(fields,
                async (reject, formData) =>
                {
                    INamedEntity? result;
                    try
                    {
                        result = callback(formData);
                    }
                    catch
                    {
                        reject("Enter valid data");
                        return;
                    }

                    var method = original != null ? HttpMethod.Patch : HttpMethod.Post;
                    var url = original != null ? "update" : "add";
                    await App.Backend.AuthorizedRequest<T>(new($"{_entityName}/{url}", method)
                    {
                        Content = WebService.ToJson(result!),
                        OnSuccess = entity => Dispatcher.UIThread.Invoke(() =>
                        {
                            if (original != null)
                            {
                                CurrentCollection!.Replace(original, result);
                                return;
                            }
                            CurrentCollection!.Add(entity!);
                        }),
                        OnFailed = () => reject("An error has occurred")
                    });
                }), title
        );
    }
    
    private void OnEntityAdding()
    {
        switch ((EEntitySection)sectionId)
        {
            case EEntitySection.None:
                AddOrUpdate<Department>("Adding a department",
                    [("Name", new TextBox { Watermark = "New department" })],
                    o => new Department { Name = o[0]!.ToString()! });
                break;
            case EEntitySection.Department:
                AddOrUpdate<Speciality>("Adding a speciality",
                    [("Name", new TextBox { Watermark = "New speciality" })],
                    o => new Speciality
                    {
                        Name = o[0]!.ToString()!, DepartmentId  = _selectedDept!.Id
                    });
                break;
            case EEntitySection.Speciality:
                AddOrUpdate<Group>("Adding a group", [
                        ("Name", new TextBox { Watermark = "New group" }),
                        ("Year (course)", new TextBox { Watermark = "1" })
                    ],
                    o => new Group
                    {
                        Name = o[0]!.ToString()!, Year  = Convert.ToInt32(o[1]!),
                        SpecialityId = _selectedSpeciality!.Id
                    });
                break;
        }
    }

    private async Task OnEntityRemoving(INamedEntity entity)
    {
        await App.Backend.AuthorizedRequest<Unit>(new($"{_entityName}/delete?id={entity.Id}", HttpMethod.Delete)
        {
            OnSuccess = _ => Dispatcher.UIThread.Invoke(() => CurrentCollection!.Remove(entity)),
        });
    }
    
    // Hardcode 
    private void OnEntityEditing(INamedEntity entity)
    {
        switch ((EEntitySection)sectionId)
        {
            case EEntitySection.None:
                AddOrUpdate<Department>("Editing department",
                    [("Name", new TextBox { Watermark = "New department", Text = entity.Name })],
                    o => new Department { Name = o[0]!.ToString()!, Id = entity.Id },entity);
                break;
            case EEntitySection.Department:
                var speciality = entity as Speciality;
                AddOrUpdate<Speciality>("Editing a speciality",
                    [("Name", new TextBox { Watermark = "New speciality", Text = entity.Name })],
                    o => new Speciality
                    {
                        Name = o[0]!.ToString()!, DepartmentId  = speciality!.DepartmentId, Id = speciality.Id
                    },entity);
                break;
            case EEntitySection.Speciality:
                AddOrUpdate<Group>("Editing a group", [
                        ("Name", new TextBox { Watermark = "New group", Text= entity.Name }),
                        ("Year (course)", new TextBox { Watermark = "1", Text = (entity as Group)!.Year.ToString() })
                    ],
                    o => new Group
                    {
                        Name = o[0]!.ToString()!, Year  = Convert.ToInt32(o[1]!),
                        SpecialityId = (entity as Group)!.SpecialityId,
                        Id = entity.Id
                    },entity);
                break;
        }
    }
    
    private void OnEntitySelection(INamedEntity entity)
    {
        SetSection((EEntitySection)(sectionId + 1), entity);
    }

    private void BreadcrumbChanged(string key)
    {
        var index = Breadcrumb.IndexOf(key);
        if (index == sectionId)
        {
            return;
        }

        SetSection((EEntitySection)index,null);
    }
    
    public AdminViewModel()
    {
        AddEntityCommand = ReactiveCommand.Create(OnEntityAdding);
        EditCommand = ReactiveCommand.Create<INamedEntity>(OnEntityEditing);
        RemoveCommand = ReactiveCommand.CreateFromTask<INamedEntity>(OnEntityRemoving);
        EntitySelectCommand = ReactiveCommand.Create<INamedEntity>(OnEntitySelection);
        BreadcrumbCommand = ReactiveCommand.Create<string>(BreadcrumbChanged);
        _ = SetSection(EEntitySection.None,null);
    }
}