using System;

namespace SchedulePlanner.Tools.Backend;

internal static class Endpoints
{
    public static readonly Uri Base = new("http://localhost:5254/api/v1/");
    public const string Login = "auth/login";
    public const string Register = "auth/register";
    public const string RefreshSession = "auth/session/refresh";
    public const string TerminateSession = "auth/session/terminate";
    
    // Departments
    public const string AllDeps = "department/all";
    public const string AddDept = "department/add";
}