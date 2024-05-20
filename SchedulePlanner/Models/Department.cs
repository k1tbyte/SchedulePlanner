namespace SchedulePlanner.Models;

public sealed class Department : INamedEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }
}