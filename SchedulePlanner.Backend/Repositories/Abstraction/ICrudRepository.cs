namespace SchedulePlanner.Backend.Repositories.Abstraction;

public interface ICrudRepository<T>
{
    public void Add(T entity, bool save);

    public T? Get(object id);

    public void Update(T entity, bool save);

    public bool DeleteById(object id, bool save);
    public bool Delete(T entity, bool save);
}