using Microsoft.AspNetCore.Mvc;
using SchedulePlanner.Backend.Controllers.Abstraction;
using SchedulePlanner.Backend.Data;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Repositories;
using SchedulePlanner.Backend.Repositories.Abstraction;

namespace SchedulePlanner.Backend.Controllers;

[Route(App.RoutePattern)]
//[RequireAccessRights(UserAccessRights.Admin)]
public sealed class DepartmentController(IDepartmentRepository repository) 
    : BaseCrudController<Department>(repository)
{
    [HttpGet]
    public List<Department> All() => repository.All();
}