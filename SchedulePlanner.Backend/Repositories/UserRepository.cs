using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Backend.Data;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Repositories.Abstraction;
using SchedulePlanner.Backend.Services;
using SchedulePlanner.Backend.Tools;

namespace SchedulePlanner.Backend.Repositories;

public class UserRepository(AppDbContext context, JwtService jwtService) : IUserRepository
{
    private const int MaxSessions = 5;
    public DbSet<User> Users => context.Users;
    
    /// <summary>
    /// Creating a new session for a user
    /// </summary>
    public (string accessToken, Guid refreshToken) CreateSession(User user)
    {
        if (user.Sessions.Count >= MaxSessions)
        {
            context.Sessions.RemoveRange(
                user.Sessions.OrderBy(o => o.Expires)
                .Take(user.Sessions.Count - (MaxSessions - 1) ));
        }

        var token = jwtService.CreateToken(user);
        context.Sessions.Add(new Session
        {
            Expires = DateTimeOffset.UtcNow.AddDays(JwtService.RefreshTokenLifetime).ToUnixTimeSeconds(),
            UserId = user.Id,
            Token = token.refreshToken
        });

        context.SaveChanges();
        return token;
    }
    
    public (string, Guid)? RefreshSession(Guid refreshToken)
    {
        if (refreshToken == default)
            return null;
        
        var session = context.Sessions.Include(o => o.User)
            .FirstOrDefault(o => o.Token == refreshToken);
        
        // Invalid refresh token or expired -> need to auth again
        if (session == null || session.Expires <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            return null;
        
        context.Remove(session);
        return CreateSession(session.User);
    }
    

    public async Task<User?> Register(string username, string password)
    {
        if (await context.Users.AnyAsync(o => o.Username == username).ConfigureAwait(false))
            return null;

        var user = new User
        {
            Username = username,
            Password = PasswordManager.HashPassword(password, out var salt, 16),
            PasswordSalt = salt,
        };
        
        await context.AddAsync(user).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return user;
    }
}