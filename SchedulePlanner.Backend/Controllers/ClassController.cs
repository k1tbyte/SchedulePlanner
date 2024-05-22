using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Backend.Controllers.Abstraction;
using SchedulePlanner.Backend.Data;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Repositories.Abstraction;

namespace SchedulePlanner.Backend.Controllers;


[Route(App.RoutePattern)]
public sealed class ClassController(ICrudRepository<UniversityClass> repository, AppDbContext context)
    : BaseCrudController<UniversityClass>(repository)
{
    [HttpGet]
    public UniversityClass[] GetByDay(int groupId, DayOfWeek dayOfWeek)
    {
        return context.Classes.FromSql($"""
                                        SELECT "class".*
                                        FROM "class"
                                                 JOIN "groups_classes" gc ON "class".id = gc.class_id 
                                                                                 AND "class".day_of_week = {dayOfWeek}
                                        WHERE gc.group_id = {groupId};
                                        """).ToArray();
    }

#if !DEBUG
    [RequireAccessRights(UserAccessRights.Admin)]
#endif
    public override UniversityClass Add(UniversityClass entity)
    {
        var universityClass = base.Add(entity);
        context.Database.ExecuteSql($"""
                                        INSERT INTO "groups_classes" (group_id, class_id)
                                        VALUES ({entity.GroupId}, {universityClass.Id})
                                        """);
        universityClass.GroupId = entity.GroupId;
        return universityClass;
    }
}