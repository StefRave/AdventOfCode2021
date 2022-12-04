using System.ComponentModel;

namespace AdventOfCode2022;

public class Day04 : IAdvent
{
    public void Run()
    {
        (int from, int to)[][] input = Advent.ReadInputLines()
            .Select(l => GetOrderedPairs(l.Split(',').Select(p => p.Split('-').Select(int.Parse).ToArray())))
            .ToArray();

        var fullyContains = input.Where(l => l[0].to >= l[1].to).Count();
        Advent.AssertAnswer1(fullyContains, expected: 503, sampleExpected: 2);

        var overlap = input.Where(l => l[0].to >= l[1].from).Count();
        Advent.AssertAnswer2(overlap, expected: 827, sampleExpected: 4);
    }

    private static (int from, int to)[] GetOrderedPairs(IEnumerable<int[]> pairs) 
        => pairs.Select(a => (from: a[0], to: a[1]))
            .OrderBy(p => p.from)
            .ThenByDescending(p => p.to)
            .ToArray();
}