namespace SchedulePlanner.Models;

public class Speciality : INamedEntity
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public string Name { get; set; }
}