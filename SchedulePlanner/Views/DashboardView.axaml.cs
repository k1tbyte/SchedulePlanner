using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SchedulePlanner.ViewModels;

namespace SchedulePlanner.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        Greetings.Text = $"You are logged in as:\n{App.Backend.CurrentSession?.JwtAccess?.Claims.First(o => o.Type == "username").Value}";
    }
}