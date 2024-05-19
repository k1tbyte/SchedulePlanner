using Microsoft.AspNetCore.Mvc;
using SchedulePlanner.Backend.Repositories;
using SchedulePlanner.Backend.Repositories.Abstraction;

namespace SchedulePlanner.Backend.Controllers.Abstraction;

[ApiController]
public abstract class BaseCrudController<T>(ICrudRepository<T> repository)
{
    [HttpPost]
    public virtual void Add([FromBody] T entity)
    {
        repository.Add(entity,true);
    }

    [HttpGet]
    public virtual T? Get(int id)
    {
        return repository.Get(id);
    }

    [HttpPatch]
    public virtual void Update([FromBody] T entity)
    {
        repository.Update(entity,true);
    }

    [HttpDelete]
    public virtual void Delete(int id)
    {
        repository.DeleteById(id,true);
    }
}