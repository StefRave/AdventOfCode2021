namespace AdventOfCode2023.Helpers;

public static class StringExtensions
{
    public static string[] SplitByNewLine(this string input)
        => input.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
    public static string[] SplitByDoubleNewLine(this string input)
        => input.Split(new string[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

    public static int[] GetNumbers(this string line)
        => Regex.Matches(line, @"\d+").Select(m => int.Parse(m.Value)).ToArray();

    public static long[] GetLongs(this string line)
        => Regex.Matches(line, @"\d+").Select(m => long.Parse(m.Value)).ToArray();
}