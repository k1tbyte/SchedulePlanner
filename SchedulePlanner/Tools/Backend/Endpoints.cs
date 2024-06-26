using System;

namespace SchedulePlanner.Tools.Backend;

internal static class Endpoints
{
    public static readonly Uri Base = new($"{App.Settings.ApiUrl.TrimEnd('/')}/api/v1/");
    public const string Login = "auth/login";
    public const string Register = "auth/register";
    public const string RefreshSession = "auth/session/refresh";
    public const string TerminateSession = "auth/session/terminate";
    
    // Departments
    public const string Department = "department";
    public const string DepartmentAll = $"{Department}/all";
    public const string DepartmentAdd = $"{Department}/add";
    
    // Specialities
    public const string Speciality = "speciality";
    public const string SpecialityAll = $"{Speciality}/all?departmentId=";
    public const string SpecialityAdd = $"{Speciality}/add";
    
    // Groups
    public const string Group = "group";
    public const string GroupAll = $"{Group}/all?specialityId=";
    public const string GroupAdd = $"{Group}/add";
    
    // Student
    public const string Student = "student";
    public const string StudentSearch = $"{Student}/search?";
    public const string StudentAddGroup = $"{Student}/addtogroup?";
    
    // Class
    public const string Class = "class";
    public const string ClassAdd = $"{Class}/add";
    public const string ClassUpdate = $"{Class}/update";
    public const string ClassGetByDay = $"{Class}/getbyday?";
}