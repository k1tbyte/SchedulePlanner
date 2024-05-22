using System;

namespace SchedulePlanner.Models;

public sealed class UniversityClass
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string SubjectName { get; set; }
    public string TeacherName { get; set; }
    public string ClassroomName { get; set; }
    public TimeOnly StartsAt { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public int Duration { get; set; }
    public TimeOnly EndAt => StartsAt.AddMinutes(Duration);
}