using Microsoft.AspNetCore.Mvc;
using SchedulePlanner.Backend.Controllers.Abstraction;
using SchedulePlanner.Backend.Data;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Repositories.Abstraction;

namespace SchedulePlanner.Backend.Controllers;

[Route(App.RoutePattern)]
public sealed class GroupController (ICrudRepository<Group> repository, AppDbContext context)  
    : BaseCrudController<Group>(repository)
{
    [HttpGet]
    public Group[] All(int specialityId) => 
        context.Groups.Where(o => o.SpecialityId == specialityId).ToArray();
    
}