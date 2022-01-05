namespace AdventOfCode2018;

public class Day06 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => line.Split(", ").Select(int.Parse).ToArray())
            .Select((s, i) => (x: s[0], y: s[1], id: i, dist: 0))
            .ToArray();

        int maxX = input.Max(i => i.x);
        int maxY = input.Max(i => i.y);

        var dict = new Dictionary<(int x, int y), (int id, int dist)>();

        var queue = new Queue<(int x, int y, int id, int dist)>(input);
        while (true)
        {
            var (x, y, id, dist) = queue.Dequeue();
            if (dist > 150)
                break;

            if (dict.TryGetValue((x, y), out var val))
            {
                if (val.dist == dist && val.id != -1 && val.id != id)
                    dict[(x, y)] = (-1, dist); // multiple candidates at same distance
                continue;
            }
            dict[(x, y)] = (id, dist);

            queue.Enqueue((x - 1, y, id, dist + 1));
            queue.Enqueue((x + 1, y, id, dist + 1));
            queue.Enqueue((x, y - 1, id, dist + 1));
            queue.Enqueue((x, y + 1, id, dist + 1));
        }
        int maxDist = dict.Max(kv => kv.Value.dist);
        var idsAtEdge = dict.Where(kv => kv.Value.dist == maxDist).Select(kv => kv.Value.id).ToHashSet();
        var idSize = dict
            .Where(kv => !idsAtEdge.Contains(kv.Value.id))
            .GroupBy(kv => kv.Value.id)
            .Select(g => (id: g.Key, cnt: g.Count()))
            .OrderByDescending(i => i.cnt)
            .ToArray();
        Advent.AssertAnswer1(idSize[0].cnt, expected: 3907, sampleExpected: 17);


        int maxDistance = Advent.UseSampleData ? 32 : 10000;
        var dictX = Enumerable.Range(-100, 600).ToDictionary(x => x, x => input.Sum(s => Math.Abs(x - s.x)));
        var dictY = Enumerable.Range(-100, 600).ToDictionary(y => y, y => input.Sum(s => Math.Abs(y - s.y)));
        int count = 0;
        for (int y = -100; y < 500; y++)
            for (int x = -100; x < 500; x++)
                count += dictX[x] + dictY[y] < maxDistance ? 1 : 0;

        Advent.AssertAnswer2(count, expected: 42036, sampleExpected: 16);
    }
}