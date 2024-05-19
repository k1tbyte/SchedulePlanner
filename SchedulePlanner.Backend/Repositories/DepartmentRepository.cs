using SchedulePlanner.Backend.Data;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Repositories.Abstraction;

namespace SchedulePlanner.Backend.Repositories;

public class DepartmentRepository(AppDbContext context) :
    BaseCrudRepository<Department>(context, context.Departments), IDepartmentRepository
{
    public List<Department> All() => context.Departments.ToList();
}