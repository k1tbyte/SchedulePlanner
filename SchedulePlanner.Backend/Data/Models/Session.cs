using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulePlanner.Backend.Data.Models;

[Table("session")]
public sealed class Session
{
    [Key]
    [Column("refresh_token")]
    public required Guid Token { get; init; }
        
    [Column("expires")]
    public required long Expires { get; init; }
        
    [Column("user_id")]
    public required long UserId { get; init; }

    public User User { get; set; } = null!;
}