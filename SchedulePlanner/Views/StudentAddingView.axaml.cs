using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SchedulePlanner.Models;
using SchedulePlanner.ViewModels;

namespace SchedulePlanner.Views;

public partial class StudentAddingView : UserControl
{
    public StudentAddingView(Group group)
    {
        InitializeComponent();
        this.DataContext = new StudentAddingViewModel(group);
    }
}