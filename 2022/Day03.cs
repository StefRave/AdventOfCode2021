namespace AdventOfCode2022;

public class Day03 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(l => new[] { l.Substring(0, l.Length / 2), l.Substring(l.Length / 2) })
            .ToArray();
        int answer1 = GetIntersectionReturnSum(input);
        Advent.AssertAnswer1(answer1, expected: 7674, sampleExpected: 157);

        input = Advent.ReadInputLines()
            .Chunk(3)
            .ToArray();
        int answer2 = GetIntersectionReturnSum(input);
        Advent.AssertAnswer2(answer2, expected: 2805, sampleExpected: 70);
    }

    private static int GetIntersectionReturnSum(string[][] input)
    {
        var intersect = input
            .Select(l => l.Aggregate((a, b) => new string(a.Intersect(b).ToArray()))
                .Select(c => c <= 'Z' ? (c - 'A' + 27) : (c - 'a' + 1)).ToArray())
            .ToArray();
        return intersect.Sum(l => l[0]);
    }
}