using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SchedulePlanner.Backend.Data.Models;

namespace SchedulePlanner.Backend.Services;

public sealed class JwtService(IConfiguration config)
{
    /// <summary>
    /// Minutes
    /// </summary>
    /// 
    public const int AccessTokenLifetime = 10;
    /// <summary>
    /// Days
    /// </summary>
    public const int RefreshTokenLifetime = 30;

    public const string AccessRightsClaimName = "accessRights";
    public const string UserIdClaimName = "userId";
    
    private string GenerateAccessToken(List<Claim> claims)
    {
        var lifeTime = TimeSpan.FromMinutes(AccessTokenLifetime);
        
        var key = Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!);

        var token = new JwtSecurityToken(
            claims: claims,
            issuer: config["JwtSettings:Issuer"],
            expires: DateTime.UtcNow.Add(lifeTime),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string accessToken, Guid refreshToken) CreateToken(User user) =>
        (GenerateAccessToken([
            new Claim(UserIdClaimName, user.Id.ToString()),
            new Claim(AccessRightsClaimName, ((byte)user.AccessRights).ToString())
        ]), Guid.NewGuid());
}