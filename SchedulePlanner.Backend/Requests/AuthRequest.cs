namespace SchedulePlanner.Backend.Requests;

public sealed class AuthRequest
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}