using System.Linq;

namespace AdventOfCode2024;

public class Day25 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByDoubleNewLine();
        var locks = new List<int[]>();
        var keys = new List<int[]>();

        foreach (string inp in input)
        {
            var lines = inp.SplitByNewLine();
            if (lines[0].All(c => c == '#'))
                keys.Add(GetPattern(lines[1..]));
            else if (lines[^1].All(c => c == '#'))
                locks.Add(GetPattern(lines[..^1]));
        }
        
        int answer1 = 
            (
                from int[] k in keys
                from int[] l in locks
                where k.Zip(l).All(a => a.First + a.Second <= 5)
                select 1
            ).Count();
        Advent.AssertAnswer1(answer1, expected: 3284, sampleExpected: 3);
    }
    static int[] GetPattern(string[] lines)
    {
        return Enumerable.Range(0, lines[0].Length)
            .Select(i => lines.Count(line => line[i] == '#'))
            .ToArray();
    }
}
