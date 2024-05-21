using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace SchedulePlanner.Models;

public sealed class Student : INotifyPropertyChanged
{
    private bool _isSameGroup;
    public string Email { get; init; }
    public string? GroupName { get; init; }
    public int? GroupId { get; set; }
    public int UserId { get; set; }

    [JsonIgnore]
    public bool IsSameGroup
    {
        get => _isSameGroup;
        set
        {
            _isSameGroup = value;
            OnPropertyChanged();
        }
    }

    public override string ToString()
    {
        return GroupName == null ? Email : $"{Email} | {GroupName}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) 
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}