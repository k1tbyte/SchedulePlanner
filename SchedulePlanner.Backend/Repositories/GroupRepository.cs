using SchedulePlanner.Backend.Data;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Repositories.Abstraction;

namespace SchedulePlanner.Backend.Repositories;

public class GroupRepository(AppDbContext context)  : 
    BaseCrudRepository<Group>(context, context.Groups);