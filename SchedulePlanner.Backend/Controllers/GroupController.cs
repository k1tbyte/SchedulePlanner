using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
#if !DEBUG
    [RequireAccessRights(UserAccessRights.Admin)]
#endif
    public Group[] All(int specialityId) => 
        context.Groups.Where(o => o.SpecialityId == specialityId).ToArray();
    

    [HttpGet]
    public Group? GetByUsername(string username)
    {
        var group = context.Users.Include(o => o.Group)
            .FirstOrDefault(o => o.Username == username.ToLower())?.Group;

        // Preventing recursion during serialization
        return group == null ? null : new Group()
        {
            Name = group.Name,
            Year = group.Year,
            SpecialityId = group.SpecialityId,
            Id = group.Id,
            Students = null!
        };
    }
}