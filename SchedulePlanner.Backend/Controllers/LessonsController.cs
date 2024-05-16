using Microsoft.AspNetCore.Mvc;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Tools.Attributes;

namespace SchedulePlanner.Backend.Controllers;

[Route(App.RoutePattern)]
[RequireAccessRights(UserAccessRights.Admin)]
public sealed class LessonsController
{
    [HttpGet]
    public string All()
    {
        return "1";
    }
}