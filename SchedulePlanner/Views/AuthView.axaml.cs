using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SchedulePlanner.ViewModels;

namespace SchedulePlanner.Views;

public partial class AuthView : StackPanel
{
    public AuthView()
    {
        InitializeComponent();
        this.DataContext = new AuthViewModel();
    }
}