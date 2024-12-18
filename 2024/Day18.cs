namespace AdventOfCode2024;

public class Day18 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine()
            .Select(l => l.Split(',').Select(int.Parse).ToArray())
            .Select(l => new V2(l[0], l[1]))
            .ToArray();

        var toTake1 = Advent.UseSampleData ? 12 : 1024;
        var width = Advent.UseSampleData ? 6 : 70;
        
        int answer1 = ShortestPath(input.Take(toTake1), width);
        Advent.AssertAnswer1(answer1, expected: 372, sampleExpected: 22);

        string answer2 = "";
        for (int i = toTake1; i < input.Length; i++)
        {
            var length = ShortestPath(input.Take(i), width);
            if (length < 0)
            {
                answer2 = $"{input[i-1].x},{input[i-1].y}";
                break;
            }
        }
        Advent.AssertAnswer2(answer2, expected: "25,6", sampleExpected: "6,1");
    }

    private int ShortestPath(IEnumerable<V2> enumerable, int width)
    {
        var bytes = new HashSet<V2>();
        foreach (var v2 in enumerable)
            bytes.Add(v2);

            V2 start = (0, 0);
        V2 end = (width, width);
        var q = new Queue<(V2 p, int d)>();
        var visited = new HashSet<V2>();
        q.Enqueue((start, 0));
        while (q.Count > 0)
        {
            var (p, steps) = q.Dequeue();
            if (p == end)
                return steps;
            if (visited.Contains(p))
                continue;
            visited.Add(p);
            foreach (var d in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
            {
                var np = p + d;
                if (np.x < 0 || np.y < 0 || np.x > end.x || np.y > end.y)
                    continue;
                if (bytes.Contains(np))
                    continue;
                q.Enqueue((np, steps + 1));
            }
        }
        return -1;
    }
}