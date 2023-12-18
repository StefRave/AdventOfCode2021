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
            GetVectors(input.Select(l => (Enum.Parse<Movement>(l.m), l.len))));
        Advent.AssertAnswer1(answer1, expected: 28911, sampleExpected: 62);

        var toVector = (string s) => ((Movement)(s[^1] - '0'), long.Parse(s[..^1], System.Globalization.NumberStyles.HexNumber));
        var answer2 = CalculateAreaPicks(
            GetVectors(input.Select(l => toVector(l.color))));
        Advent.AssertAnswer2(answer2, expected: 77366737561114, sampleExpected: 952408144115);
    }

    private static long CalculateAreaPicks((long x, long y)[] vectors)
    {
        double area = 0.0;
        int j = vectors.Length - 1;
        long circumference = 0;
        var prev = vectors[^1];
        for (int i = 0; i < vectors.Length; i++)
        {
            area += (vectors[j].x + vectors[i].x) * (vectors[j].y - vectors[i].y);
            j = i;
            circumference += Math.Abs(prev.x - vectors[i].x + prev.y - vectors[i].y);
            prev = vectors[i];
        }
        return (long)Math.Abs(area / 2.0) + (circumference / 2 + 1);
    }

    private static (long x, long y)[] GetVectors(IEnumerable<(Movement, long len)> input)
    {
        var vectors = new List<(long x, long y)>();
        var p = (x: 0L, y: 0L);
        foreach (var (m, len) in input)
        {
            var (x, y) = m switch
            {
                Movement.R => (len, 0L),
                Movement.L => (-len, 0),
                Movement.U => (0, -len),
                Movement.D => (0, len),
                _ => throw new NotImplementedException(),
            };
            p = (x: p.x + x, y: p.y + y);
            vectors.Add(p);
        }
        return vectors.ToArray();
    }
}
