using System.Text.RegularExpressions;

namespace SchedulePlanner.Tools;

public static partial class Regexes
{
    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex EmailRegex();

    public static readonly Regex Email = EmailRegex();
    
    [GeneratedRegex(@"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$")]
    private static partial Regex PasswordRegex();
    public static readonly Regex Password = PasswordRegex();

    public static bool IsValid(this Regex regex, string? value)
        => value != null && regex.IsMatch(value);

}