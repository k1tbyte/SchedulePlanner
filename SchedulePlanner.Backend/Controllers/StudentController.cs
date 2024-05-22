using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Backend.Data;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Dto;

namespace SchedulePlanner.Backend.Controllers;

[Route(App.RoutePattern)]
#if !DEBUG
    [RequireAccessRights(UserAccessRights.Admin)]
#endif
public sealed class StudentController(AppDbContext context)
{
    [HttpGet]
    public StudentDto[] Search(string email)
    {
        return context.Users.Include(o => o.Group)
            .Where(o => EF.Functions.ILike(o.Username,$"%{email}%"))
            .Select(o => new StudentDto(o.Username,o.Id, o.Group))
            .ToArray();
    }
    
    [HttpGet]
    public void AddToGroup(int studentId, int? groupId)
    {
        var student = context.Users.Find(studentId);
        student!.GroupId = groupId;
        context.Users.Update(student);
        context.SaveChanges();
    }
}