using SchedulePlanner.Backend.Data.Models;

namespace SchedulePlanner.Backend.Dto;

public class StudentDto
{
    public StudentDto(string username, int userId, Group? group)
    {
        Email = username;
        UserId = userId;
        if (group == null)
            return;
        GroupId = group.Id;
        GroupName = group.Name;
    }
    public  string Email { get; init; }
    public  int UserId { get; init; }
    public string? GroupName { get; init; }
    public int? GroupId { get; init; }
}