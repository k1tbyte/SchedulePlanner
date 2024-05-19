using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulePlanner.Backend.Data.Models;

[Table("speciality")]
public class Speciality
{
    [Key]
    [Column("id")]
    public int Id { get; init; }
    
    [Column("name")]
    public required string Name { get; init; }
    
    [Column("department_id")]
    public required int DepartmentId { get; init; }
}