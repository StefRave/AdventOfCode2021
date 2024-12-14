namespace AdventOfCode2024;

public class Day14 : IAdvent
{
    public void Run()
    {
        (V2 p, V2 v)[] input = Advent.ReadInput().SplitByNewLine()
            .Select(x => x.Split(" ").Select(v => v[2..].Split(',').Select(int.Parse).ToArray()).ToArray())
            .Select(l => (p: new V2(l[0][0], l[0][1]), new V2(l[1][0], l[1][1])))
            .ToArray();

        V2 size = Advent.UseSampleData ? new V2(11, 7) : new V2(101, 103);
        V2[] newPos = DoIt(input, size, 100);

        var q = new int[4];
        foreach (var p in newPos.Where(p => p.x != size.x / 2 && p.y != size.y / 2))
            q[(p.x < size.x / 2 ? 0 : 1) + (p.y < size.y / 2 ? 0 : 2)] += 1;
        long answer1 = q.Aggregate(1L, (a, b) => a * b);
        Advent.AssertAnswer1(answer1, expected: 221142636, sampleExpected: 12);

        if (Advent.UseSampleData)
            return;

        int answer2 = 0;
        for (int i = 1; i < 10000000; i++)
        {
            var g = DoIt(input, size, i).GroupBy((V2 k) => k).ToDictionary(g => g.Key, g => g.Count());
            int count8 = 0;
            Console.Write($"{i} \r");
            foreach (var (k, c) in g)
            {
                int count = 0;
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                        if (x != 0 || y != 0)
                            count += g.ContainsKey(k + new V2(x, y)) ? 1 : 0;
                if (count > 7)
                {
                    count8++;
                }
            }
            if (count8 > 3)
            {
                Console.Clear();
                Console.WriteLine(i);
                for (int y = 0; y < size.y; y++)
                {
                    for (int x = 0; x < size.x; x++)
                    {
                        var c = g.GetValueOrDefault(new V2(x, y));
                        if (c > 9)
                            Console.Write('X');
                        else
                            Console.Write(c == 0 ? '.' : c.ToString());
                    }
                    Console.WriteLine();
                }
                answer2 = i;
                break;
            }
        }
        Advent.AssertAnswer2(answer2, expected: 7916, sampleExpected: 0);


        static V2[] DoIt((V2 p, V2 v)[] input, V2 size, int iterations)
        {
            return input.Select(r =>
            {
                V2 p = r.p;
                V2 v = r.v;
                return new V2((p.x + ((v.x + size.x) * iterations)) % size.x, (p.y + ((v.y + size.y) * iterations)) % size.y);
            }).ToArray();
        }
    }
}

public record V2(int x, int y)
{
    public static V2 operator -(V2 a, V2 b) => new V2(a.x - b.x, a.y - b.y);
    public static V2 operator +(V2 a, V2 b) => new V2(a.x + b.x, a.y + b.y);
}








