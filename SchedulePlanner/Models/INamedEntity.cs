namespace SchedulePlanner.Models;

public interface INamedEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}