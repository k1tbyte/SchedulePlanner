using Microsoft.AspNetCore.Mvc;
using SchedulePlanner.Backend.Controllers.Abstraction;
using SchedulePlanner.Backend.Data;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Repositories;
using SchedulePlanner.Backend.Repositories.Abstraction;
using SchedulePlanner.Backend.Tools.Attributes;

namespace SchedulePlanner.Backend.Controllers;

[Route(App.RoutePattern)]
#if !DEBUG
[RequireAccessRights(UserAccessRights.Admin)]
#endif
public sealed class SpecialityController(ICrudRepository<Speciality> repository, AppDbContext context)  
    : BaseCrudController<Speciality>(repository)
{
    [HttpGet]
    public Speciality[] All(int departmentId) => 
        context.Specialities.Where(o => o.DepartmentId == departmentId).ToArray();
}