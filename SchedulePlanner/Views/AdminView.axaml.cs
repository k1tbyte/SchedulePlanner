using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using SchedulePlanner.Models;
using SchedulePlanner.ViewModels;

namespace SchedulePlanner.Views;

public partial class AdminView : UserControl
{
    private AdminViewModel viewModel;
    public AdminView()
    {
        InitializeComponent();
        this.DataContext = viewModel = new AdminViewModel();
    }

    private void EntityCardPressed(object? sender, PointerPressedEventArgs e)
    {
        viewModel.EntitySelectCommand.Execute(
            ((sender as Border)!.DataContext as INamedEntity)!
        );
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        viewModel.BreadcrumbCommand.Execute(
            ((sender as Border)!.DataContext as string)!
        );
    }
}