using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulePlanner.Backend.Data.Models;

public enum UserAccessRights
{
    User,
    Admin
}

[Table("user")]
public sealed class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("user_id")]
    public long Id { get; set; }
    
    [Column("username")]
    public required string Username { get; init; }
    
    [Column("password")]
    public required string Password { get; init; }
    
    [Column("password_salt")]
    public required string PasswordSalt { get; init; }
    
    [Column("access_rights")] 
    public UserAccessRights AccessRights { get; init; }

    public ICollection<Session> Sessions { get; init; } = new List<Session>();
}