using Avalonia.Controls;
using Avalonia.Input;
using SchedulePlanner.Models;
using SchedulePlanner.ViewModels;

namespace SchedulePlanner.Views;

public partial class ClassesView : UserControl
{
    public ClassesView(Group group)
    {
        InitializeComponent();
        this.DataContext = new ClassesViewModel(group);
    }

    private void DayOfWeekSelection(object? sender, PointerPressedEventArgs e)
    {
        (DataContext as ClassesViewModel)!.SelectDayOfWeekCommand.Execute(
            (sender as Control)!.DataContext
        );
    }
}