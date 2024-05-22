using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SchedulePlanner.Backend.Data.Models;

[Table("class")]
public sealed class UniversityClass
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("subject_name")]
    public required string SubjectName { get; init; }
    
    [Column("teacher_name")]
    public required string TeacherName { get; init; }
    
    [Column("classroom_name")]
    public required string ClassroomName { get; init; }
    
    [Column("starts_at")]
    public required TimeOnly StartsAt { get; init; }
    
    [Column("duration")]
    public required int Duration { get; init; }
    
    [Column("day_of_week")]
    public required DayOfWeek DayOfWeek { get; init; }
    
    [JsonRequired]
    [NotMapped]
    public int GroupId { get; set; }
}