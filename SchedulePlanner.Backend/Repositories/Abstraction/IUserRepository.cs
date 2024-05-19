using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Backend.Data.Models;

namespace SchedulePlanner.Backend.Repositories.Abstraction;

public interface IUserRepository
{
    public DbSet<User> Users { get; }
    public Task<User?> Register(string username, string password);
    public (string, Guid)? RefreshSession(Guid refreshToken);
    public (string accessToken, Guid refreshToken) CreateSession(User user);
}