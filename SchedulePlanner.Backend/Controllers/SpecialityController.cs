using Microsoft.AspNetCore.Mvc;
using SchedulePlanner.Backend.Controllers.Abstraction;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Repositories;
using SchedulePlanner.Backend.Repositories.Abstraction;

namespace SchedulePlanner.Backend.Controllers;

[Route(App.RoutePattern)]
//[RequireAccessRights(UserAccessRights.Admin)]
public sealed class SpecialityController(BaseCrudRepository<Speciality> repository)  
    : BaseCrudController<Speciality>(repository)
{
    
}