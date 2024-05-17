using System;
using System.Net;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SchedulePlanner.Tools;
using SchedulePlanner.Views;

namespace SchedulePlanner.ViewModels;

public sealed class AuthViewModel : ReactiveObject
{
    [Reactive]
    public string? LoginError { get; private set; }
    [Reactive]
    public string? RegisterError { get; private set; }
    [Reactive]
    public string? LoginEmail { get; set; }
    [Reactive]
    public string? LoginPassword { get; set; }
    [Reactive]
    public string? RegisterEmail { get; set; }
    [Reactive]
    public string? RegisterPassword { get; set; }
    [Reactive]
    public string? RegisterConfirmPassword { get; set; }
    public ReactiveCommand<Unit,Unit> RegisterCommand { get; }
    public ReactiveCommand<Unit,Unit> LoginCommand { get; }

    private async Task Register()
    {
        if (!Regexes.Email.IsValid(RegisterEmail))
        {
            RegisterError = "Enter valid email";
            return;
        }

        if (!Regexes.Password.IsValid(RegisterPassword))
        {
            RegisterError = "The password must be at least 8 characters long and contain 1 capital letter and 1 number";
            return;
        }

        if (RegisterPassword != RegisterConfirmPassword)
        {
            RegisterError = string.IsNullOrEmpty(RegisterConfirmPassword)
                ? "Confirm the password"
                : "Passwords mismatch";
            return;
        }
        RegisterError = null;
        var response = await App.Backend.Request<WebService.Session>(HttpMethod.Post,
            Endpoints.Register, WebService.ToJson(
                new { username = RegisterEmail, password = RegisterPassword }
            ), App.Backend.StoreSession
        );
        
        WebService.HandleResult(response,
            "Wrong email or password!",
            () => App.MainVm.SetView(new HomeView()), 
            o => {
                if (response.Code == HttpStatusCode.Conflict)
                {
                    RegisterError = "A user with the same email already exists";
                    RegisterEmail = null;
                    return;
                }
                RegisterError = o;
            }
        );
    }

    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(LoginEmail) || string.IsNullOrEmpty(LoginPassword))
        {
            return;
        }

        WebService.HandleResult(await App.Backend.Authorize(LoginEmail, LoginPassword),
            "Wrong email or password!",
            () => App.MainVm.SetView(new HomeView()), 
            (o) => {
                LoginEmail = LoginPassword = null;
                LoginError = o;
            }
        );
    }

    public AuthViewModel()
    {
        RegisterCommand = ReactiveCommand.CreateFromTask(Register);
        LoginCommand = ReactiveCommand.CreateFromTask(Login);
    }
}