using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SchedulePlanner.Backend.Data.Models;

[Table("department")]
public sealed class Department
{
    [Key]
    [Column("id")]
    public int Id { get; init; }
    
    [Column("name")]
    public required string Name { get; init; }
}