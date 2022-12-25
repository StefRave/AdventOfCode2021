using System.Numerics;

namespace AdventOfCode2022;

public class Day25 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines();

        long total = input.Sum(line => FromSnafu(line));
        string answer1 = ToSnafu(total);

        Advent.AssertAnswer1(answer1, expected: "2==221=-002=0-02-000", sampleExpected: "2=-1=0");
    }

    private static long FromSnafu(string line)
    {
        long number = 0;
        foreach (var c in line)
        {
            int d = c switch
            {
                '0' => 0,
                '1' => 1,
                '2' => 2,
                '-' => -1,
                '=' => -2,
                _ => throw new ArgumentOutOfRangeException(nameof(c))
            };
            number = 5 * number + d;
        }
        return number;
    }

    private static string ToSnafu(long number)
    {
        string result = "";
        while (number != 0)
        {
            long current = ((number + 2) % 5) - 2;
            string c = current switch
            {
                -2 => "=",
                -1 => "-",
                0 => "0",
                1 => "1",
                2 => "2",
            };
            result = c + result;
            number = (number - current) / 5;
        }
        return result;
    }
}