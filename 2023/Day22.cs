namespace AdventOfCode2023;

public class Day22 : IAdvent
{

    public void Run()
    {
        (V3, V3)[] input = Advent.ReadInput().SplitByNewLine()
            .Select(line => line.GetInts())
            .Select(line => (s: new V3(line[0], line[1], line[2]), e: new V3(line[3], line[4], line[5])))
            .OrderBy(line => line.s.Z)
            .ToArray();
        var max = input.Aggregate(new V3(0, 0, 0), (a, b) => new V3(Math.Max(a.X, b.Item2.X), Math.Max(a.Y, b.Item2.Y), Math.Max(a.Z, b.Item2.Z)));
        var field = new int[max.X + 1, max.Y + 1, max.Z + 1];

        var touching = new Dictionary<int, HashSet<int>>();
        var supporting = new Dictionary<int, HashSet<int>>();
        touching.Add(0, new HashSet<int>());
        for (int brickNr = 1; brickNr <= input.Length; brickNr++)
        {
            touching.Add(brickNr, new HashSet<int>());
            supporting.Add(brickNr, new HashSet<int>());
            var (s, e) = input[brickNr-1];
            int goDown = 0;
            bool canGoDown = true;

            while (canGoDown)
            {
                goDown++;
                if (s.Z - goDown == 0)
                {
                    touching[0].Add(brickNr);
                    supporting[brickNr].Add(0);
                    break;
                }

                for (int x = s.X; x <= e.X; x++)
                    for (int y = s.Y; y <= e.Y; y++)
                        for (int z = s.Z; z <= e.Z; z++)
                        {
                            int t = field[x, y, z - goDown];
                            if (t != 0)
                            {
                                canGoDown = false;
                                touching[t].Add(brickNr);
                                supporting[brickNr].Add(t);
                            }
                        }
            }
            goDown--;
            for (int x = s.X; x <= e.X; x++)
                for (int y = s.Y; y <= e.Y; y++)
                    for (int z = s.Z; z <= e.Z; z++)
                        field[x, y, z - goDown] = brickNr;
        }
        int answer1 = 0;
        for (int brickNr = 1; brickNr <= input.Length; brickNr++)
        {
            int critical = touching[brickNr].Count(b => supporting[b].Count == 1);
            if (critical == 0)
                answer1++;
        }
        Advent.AssertAnswer1(answer1, expected: 524, sampleExpected: 5);

        int answer2 = 0;
        for (int brickNr = 1; brickNr <= input.Length; brickNr++)
        {
            var toRemove = new HashSet<int>();
            toRemove.Add(brickNr);
            for (int j = brickNr + 1; j <= input.Length; j++)
            {
                if (supporting[j].All(b => toRemove.Contains(b)))
                    toRemove.Add(j);
            }
            answer2 += toRemove.Count - 1;
        }
        Advent.AssertAnswer2(answer2, expected: 77070, sampleExpected: 7);
    }

    public record V3(int X, int Y, int Z);
}
