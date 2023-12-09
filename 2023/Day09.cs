
namespace AdventOfCode2023;

public class Day09 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.GetLongs())
            .ToArray();

        var answer1 = input.Select(line => DoIt(line, part: 1)).Sum();
        Advent.AssertAnswer1(answer1, expected: 1930746032, sampleExpected: 114);
        
        var answer2 = input.Select(line => DoIt(line, part: 2)).Sum();
        Advent.AssertAnswer2(answer2, expected: 1154, sampleExpected: 2);
    }

    private long DoIt(long[] line, int part)
    {
        if (line.All(v => v == 0))
            return 0;

        long[] l = new long[line.Length - 1];
        for (int i = 0; i < l.Length; i++)
            l[i] = line[i + 1] - line[i];

        if (part == 1)
            return line[^1] + DoIt(l, part);
        return line[0] - DoIt(l, part);
    }
}
