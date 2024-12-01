namespace AdventOfCode2024.Helpers;

public static class RegexExtensions
{
    public static string[] AsArray(this GroupCollection gc) => gc.Cast<Group>().Select(g => g.Value).ToArray();
}
