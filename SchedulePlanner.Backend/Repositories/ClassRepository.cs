using Microsoft.EntityFrameworkCore;
using SchedulePlanner.Backend.Data;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Repositories.Abstraction;

namespace SchedulePlanner.Backend.Repositories;

public sealed class ClassRepository(AppDbContext context) :
    BaseCrudRepository<UniversityClass>(context, context.Classes);