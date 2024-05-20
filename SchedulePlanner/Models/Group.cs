namespace SchedulePlanner.Models;

public class Group : INamedEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Year { get; set; }
    public int SpecialityId { get; set; }
}