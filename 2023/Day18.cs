namespace AdventOfCode2023;

public class Day18 : IAdvent
{
    enum Movement { R, D, L, U };
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.Split(' '))
            .Select(line => (m: line[0], len: long.Parse(line[1]), color: line[2][2..^1]))
            .ToArray();

        var answer1 = CalculateAreaPicks(
            GetVectors(input.Select(l => (Enum.Parse<Movement>(l.m), l.len))).ToArray());
        Advent.AssertAnswer1(answer1, expected: 28911, sampleExpected: 62);

        var toVector = (string s) => ((Movement)(s[^1] - '0'), long.Parse(s[..^1], System.Globalization.NumberStyles.HexNumber));
        var answer2 = CalculateAreaPicks(
            GetVectors(input.Select(l => toVector(l.color))).ToArray());
        Advent.AssertAnswer2(answer2, expected: 77366737561114, sampleExpected: 952408144115);
    }

    private static long CalculateAreaPicks((long x, long y)[] vectors)
    {
        double area = 0.0;
        long circumference = 0;
        var prev = vectors[^1];
        foreach (var vector in vectors)
        {
            area += (prev.x + vector.x) * (prev.y - vector.y);
            circumference += Math.Abs(prev.x - vector.x + prev.y - vector.y);
            prev = vector;
        }
        return (long)Math.Abs(area / 2.0) + (circumference / 2 + 1);
    }

    private static IEnumerable<(long x, long y)> GetVectors(IEnumerable<(Movement, long len)> input)
    {
        var p = (x: 0L, y: 0L);
        foreach (var (m, len) in input)
            yield return p = m switch
            {
                Movement.R => (p.x + len, p.y),
                Movement.L => (p.x - len, p.y),
                Movement.U => (p.x, p.y - len),
                Movement.D => (p.x, p.y + len),
                _ => throw new NotImplementedException(),
            };
    }
}
