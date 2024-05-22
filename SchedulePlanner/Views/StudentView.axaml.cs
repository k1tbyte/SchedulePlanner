using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SchedulePlanner.ViewModels;

namespace SchedulePlanner.Views;

public partial class StudentView : UserControl
{
    public StudentView()
    {
        InitializeComponent();
        this.DataContext = new StudentViewModel();
    }
}