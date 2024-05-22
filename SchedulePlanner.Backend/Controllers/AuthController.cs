using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Backend.Data;
using SchedulePlanner.Backend.Repositories.Abstraction;
using SchedulePlanner.Backend.Requests;
using SchedulePlanner.Backend.Services;
using SchedulePlanner.Backend.Tools;

namespace SchedulePlanner.Backend.Controllers;

[Route(App.RoutePattern)]
[ApiController]
public sealed class AuthController(IUserRepository userRepository, AppDbContext context) : ControllerBase
{
    private OkObjectResult OkTokenResult((string accessToken, Guid refreshToken) session) =>
        Ok(new { AccessToken = session.accessToken, RefreshToken = session.refreshToken });
        
    [HttpPost]
    public async Task<ActionResult> Login([FromBody] AuthRequest request)
    {
        var user = await userRepository.Users
            .Include(o => o.Sessions)
            .FirstOrDefaultAsync(o => request.Username.ToLower() == o.Username);
        
        if (user == null) 
            return NotFound("User with this username not found");

        if (!PasswordManager.CheckPassword(request.Password, user.PasswordSalt, user.Password))
            return Unauthorized("Invalid password");
        
        return OkTokenResult(userRepository.CreateSession(user));
    }
    
    [HttpPost,ActionName("session/refresh")]
    public ActionResult RefreshSession(Guid refreshToken)
    {
        var session = userRepository.RefreshSession(refreshToken);
        if (session == null)
            return Unauthorized("Invalid token");

        return OkTokenResult(session.Value);
    }

    [Authorize]
    [HttpDelete,ActionName("session/terminate")]
    public ActionResult TerminateSession(Guid? refreshToken)
    {
        var sql = "DELETE FROM session WHERE user_id = {0}"
                  + (refreshToken == null ? "" : " AND refresh_token = {1}");

        var userIdParam = long.Parse(HttpContext.User.Claims.First(o => o.Type == JwtService.UserIdClaimName).Value);
        context.Database.ExecuteSqlRaw(sql,userIdParam, refreshToken!);
        return Ok();
    }
    
    [HttpPost]
    public async Task<ActionResult> Register([FromBody] AuthRequest request)
    {
        var user = await userRepository.Register(request.Username.ToLowerInvariant(), request.Password).ConfigureAwait(false);
        if (user == null)
            return Conflict("User with this username already exists");

        return OkTokenResult(userRepository.CreateSession(user));
    }
    
    
}