using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SchedulePlanner.ViewModels;

namespace SchedulePlanner.Views;

public partial class AdminView : UserControl
{
    public AdminView()
    {
        InitializeComponent();
        this.DataContext = new AdminViewModel();
    }
}