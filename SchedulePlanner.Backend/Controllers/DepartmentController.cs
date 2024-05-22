using Microsoft.AspNetCore.Mvc;
using SchedulePlanner.Backend.Controllers.Abstraction;
using SchedulePlanner.Backend.Data;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Repositories;
using SchedulePlanner.Backend.Repositories.Abstraction;

namespace SchedulePlanner.Backend.Controllers;

[Route(App.RoutePattern)]
#if !DEBUG
    [RequireAccessRights(UserAccessRights.Admin)]
#endif
public sealed class DepartmentController(IDepartmentRepository repository) 
    : BaseCrudController<Department>(repository)
{
    [HttpGet]
    public Department[] All() => repository.All();
}