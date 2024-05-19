using SchedulePlanner.Backend.Data;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Repositories.Abstraction;

namespace SchedulePlanner.Backend.Repositories;

public class SpecialityRepository(AppDbContext context) 
    : BaseCrudRepository<Speciality>(context, context.Specialities);