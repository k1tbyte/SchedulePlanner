using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SchedulePlanner.ViewModels;

namespace SchedulePlanner.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
        DataContext = new HomeViewModel();
    }
}