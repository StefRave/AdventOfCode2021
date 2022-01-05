using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2018;

public class Day05 : IAdvent
{
    public void Run()
    {
        string input = Advent.ReadInput();

        string left = Colapse(input);
        Advent.AssertAnswer1(left.Length, expected: 9704, sampleExpected: 10);

        var options = "abcdefghijklmnopqrstuvw".Select(c => (c: c, left: Colapse(Regex.Replace(input, char.ToString(c), "", RegexOptions.IgnoreCase))));
        left = options.OrderBy(o => o.left.Length).First().left;
        Advent.AssertAnswer1(left.Length, expected: 6942, sampleExpected: 4);
    }

    private static string Colapse(string input)
    {
        var sb = new StringBuilder();
        int i = 0;
        while (i < input.Length)
        {
            if (sb.Length == 0)
                sb.Append(input[i++]);
            char c0 = sb[sb.Length - 1];
            char c1 = input[i];
            if ((c0 & 0x20) != (c1 & 0x20) &&
                (c0 & 0x1f) == (c1 & 0x1f))
            {
                if (sb.Length > 0)
                    sb.Length--;
            }
            else
                sb.Append(c1);
            i++;
        }
        return sb.ToString();
    }
}