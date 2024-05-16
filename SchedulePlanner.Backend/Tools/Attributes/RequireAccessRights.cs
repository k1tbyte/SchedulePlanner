using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Services;

namespace SchedulePlanner.Backend.Tools.Attributes;

[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
public sealed class RequireAccessRights(UserAccessRights rights) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        var claim = user.Claims.FirstOrDefault(o => o.Type == JwtService.AccessRightsClaimName)?.Value;
        
        if (claim == null || byte.Parse(claim) < (byte)rights)
        {
            context.Result = new ForbidResult();
        }
    }
}