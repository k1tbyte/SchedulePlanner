using SchedulePlanner.Backend.Data.Models;

namespace SchedulePlanner.Backend.Repositories.Abstraction;

public interface IDepartmentRepository : ICrudRepository<Department>
{
    public Department[] All();
}