using SchedulePlanner.Backend.Data.Models;

namespace SchedulePlanner.Backend.Repositories;

public interface IUserRepository
{
    public Task<User> GetIfExists();
}