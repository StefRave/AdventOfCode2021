#nullable enable
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AdventOfCode2019;

public class Day22 : IAdvent
{
    private static string GetInput() => File.ReadAllText(@"Input/input22.txt");

    void IAdvent.Run()
    {
        Line[] commands = Parse(GetInput());

        Advent.AssertAnswer1(Execute(commands, 2019, 10007), 3377);

        Process.GetCurrentProcess().Kill(); // FAIL to hard: Explenation is here https://codeforces.com/blog/entry/72593

        long index = 2020;
        long cardCount = 119315717514047;
        for (long i = 0; i < cardCount; i++)
        {
            foreach (var command in commands.Reverse())
            {
                index = command switch
                {
                    (true, 0) => DealIntoNewStackR(index, cardCount),
                    (true, int num) => DealWithIncrementR(index, cardCount, num),
                    (false, int num) => CutStackR(index, cardCount, num),
                };
            }
            WriteLine(index.ToString());
            if (index == 2020)
                1.ToString();
        }
        //Advent.AssertAnswer2(index, 2020);
    }

    private static long Execute(Line[] commands, long index, long cardCount)
    {
        foreach (var command in commands)
        {
            index = command switch
            {
                (true, 0) => DealIntoNewStack(index, cardCount),
                (true, int num) => DealWithIncrement(index, cardCount, num),
                (false, int num) => CutStack(index, cardCount, num),
            };
        }
        return index;
    }

    private static long CutStack(long index, long cardCount, long num)
        => (index + cardCount - num) % cardCount;

    private static long DealIntoNewStack(long index, long cardCount)
        => cardCount - index - 1;

    private static long DealWithIncrement(long index, long cardCount, int increment)
        => (index * increment) % cardCount;

    private static long CutStackR(long index, long cardCount, long num)
        => (index + cardCount + num) % cardCount;

    private static long DealIntoNewStackR(long index, long cardCount)
       => cardCount - index - 1;

    private static long DealWithIncrementR(long index, long cardCount, int increment)
        => (index * ModInverse(increment, cardCount)) % cardCount;

    static long ModInverse(long a, long m)
    {
        long m0 = m;
        long y = 0, x = 1;

        if (m == 1)
            return 0;
        while (a > 1) // Euclid's algo
            (m, a,x, y) = (a % m, m, y, x - (a / m) * y);
        if (x < 0)
            x += m0;
        return x;
    }


    private static Line[] Parse(string input)
    {
        var matches = Regex.Matches(input.ReplaceLineEndings("\n"), @"^(cut|deal) .*?([-\d]+)?$", RegexOptions.Multiline | RegexOptions.ECMAScript);
        var result = matches
            .Select(m => new Line(m.Groups[1].Value == "deal", m.Groups[2].Success ? int.Parse(m.Groups[2].Value) : 0))
            .ToArray();
        Assert.Equal(input.SplitByNewLine().Length, result.Length);
        return result;
    }

    public record Line(bool deal, int number);
}
