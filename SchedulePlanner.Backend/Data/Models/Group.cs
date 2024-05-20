using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulePlanner.Backend.Data.Models;

[Table("group")]
public sealed class Group
{
    [Key]
    [Column("id")]
    public int Id { get; init; }
    
    [Column("name")]
    public required string Name { get; init; }
    
    [Column("speciality_id")]
    public required int SpecialityId { get; init; }
    
    [Column("year")]
    public required int Year { get; init; }
}