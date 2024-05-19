using Microsoft.EntityFrameworkCore;

namespace SchedulePlanner.Backend.Repositories.Abstraction;

public class BaseCrudRepository<T>(DbContext context, DbSet<T> set) : ICrudRepository<T> where T : class
{
    public void Add(T entity, bool save = true)
    {
        set.Add(entity);
        if (save)
        {
            context.SaveChanges();
        }
    }

    public T? Get(object id) => set.Find(id);

    public void Update(T entity, bool save = true)
    {
        set.Update(entity);
        if (save)
        {
            context.SaveChanges();
        }
    }

    public bool DeleteById(object id, bool save = true) => Delete(set.Find(id));

    public bool Delete(T? entity, bool save = true)
    {
        if (entity == default)
        {
            return false;
        }
        
        set.Remove(entity);
        if (save)
        {
            context.SaveChanges();
        }
        return true;
    }
}