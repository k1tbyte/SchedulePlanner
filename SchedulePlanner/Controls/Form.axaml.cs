using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;

namespace SchedulePlanner.Controls;

public partial class Form : UserControl
{
    public bool AutoClosableModal { get; set; } = true;
    public bool AllowDefaults { get; set; } = false;
    
    private readonly Func<Action<string>,object?[], Task> _onSubmit;
    public Func<bool, object?>[] Validators { get; set; }
    
    /// <summary>
    ///  Supports <see cref="TextBox"/> <see cref="DatePicker"/>, <see cref="TimePicker"/>, <see cref="NumericUpDown"/> as form elements
    /// </summary>
    /// <param name="namedControls">Controls with text blocks for the names of form elements</param>
    /// <param name="onSubmit">Takes a reject() action to set an error and a list of form input objects</param>
    public Form((string, Control)[] namedControls, Func<Action<string>,object?[], Task> onSubmit)
    {
        _onSubmit = onSubmit;
        InitializeComponent();
        
        foreach (var pair in namedControls)
        {
            FormElements.Children.Add(new TextBlock{Text = pair.Item1});
            FormElements.Children.Add(pair.Item2);
        }
    }

    private async void Submit(object? sender, RoutedEventArgs e)
    {
        var rejected = false;
        ErrorBlock.Text = null;
        var submitBtn = sender as Button;
        submitBtn!.IsEnabled = false;
        List<object?> formData = new();
        
        foreach (var formElement in FormElements.Children)
        {
            switch (formElement)
            {
                case TextBox textBox:
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        submitBtn.IsEnabled = true;
                        ErrorBlock.Text = "Input cannot be empty";
                        return;
                    }
                    formData.Add(textBox.Text);
                    break;
                case DatePicker datePicker:
                    formData.Add(datePicker.SelectedDate);
                    break;
                case TimePicker timePicker:
                    formData.Add(timePicker.SelectedTime);
                    break;
                case NumericUpDown upDown:
                    formData.Add(upDown.Value);
                    break;
            }
        }
        
        await _onSubmit(
            error =>
            {
                rejected = true;
                Dispatcher.UIThread.Invoke(() => ErrorBlock.Text = error);
            },
            formData.ToArray()
        );

        submitBtn.IsEnabled = true;
        if (!rejected && AutoClosableModal)
        {
            ModalWindow.Close();
        }
    }
}