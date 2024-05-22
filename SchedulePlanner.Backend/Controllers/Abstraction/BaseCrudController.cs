using Microsoft.AspNetCore.Mvc;
using SchedulePlanner.Backend.Repositories;
using SchedulePlanner.Backend.Repositories.Abstraction;

namespace SchedulePlanner.Backend.Controllers.Abstraction;

[ApiController]
public abstract class BaseCrudController<T>(ICrudRepository<T> repository)
{
#if !DEBUG
    [RequireAccessRights(UserAccessRights.Admin)]
#endif
    [HttpPost]
    public virtual T Add([FromBody] T entity)
    {
        return repository.Add(entity);
    }

    [HttpGet]
    public virtual T? Get(int id)
    {
        return repository.Get(id);
    }

#if !DEBUG
    [RequireAccessRights(UserAccessRights.Admin)]
#endif
    [HttpPatch]
    public virtual void Update([FromBody] T entity)
    {
        repository.Update(entity,true);
    }

#if !DEBUG
    [RequireAccessRights(UserAccessRights.Admin)]
#endif
    [HttpDelete]
    public virtual void Delete(int id)
    {
        repository.DeleteById(id,true);
    }
}