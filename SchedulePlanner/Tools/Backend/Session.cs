using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

namespace SchedulePlanner.Tools.Backend;

public class Session
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
        
    [JsonIgnore]
    public JwtSecurityToken? JwtAccess { get; set; }
}