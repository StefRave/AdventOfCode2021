namespace AdventOfCode2023;

public class Day21 : IAdvent
{
    public (int x, int y)[] deltas = [(-1, 0), (0, -1), (0, 1), (1, 0)];

    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine();
        var (maxX, maxY) = (input[0].Length, input.Length);
        
        var (sx, sy) = (0, 0);
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[y].Length; x++)
                if (input[y][x] == 'S')
                    (sx, sy) = (x, y);

        int maxIterations = 2500;
        int totalTimes = Advent.UseSampleData ? 5000 : 26501365;
        long[] totals = new long[totalTimes + 1];
        long[] diff = new long[totalTimes + 1];
        long[] diff2 = new long[totalTimes + 1];
        var visited = new bool[maxIterations * 4, maxIterations * 4];
        var (cx, cy) = (maxIterations * 2, maxIterations * 2);
        visited[cx + sx, cy + sy] = true;
        var toGo = new Queue<(int x, int y)>([(sx, sy)]);
        var periodDetection = new Dictionary<long, (long period, long lastOccurance, int count)>();
        long period = 0;
        long times;
        totals[0] = 1;
        for (times = 1; times < maxIterations; times++)
        {
            var toGoNew = new Queue<(int x, int y)>();
            while (toGo.TryDequeue(out var pos))
            {
                foreach (var (dx, dy) in deltas)
                {
                    var (px, py) = (pos.x + dx, pos.y + dy);
                    var (mx, my) = ((px % maxX + maxX) % maxX, (py % maxY + maxX) % maxY);
                    if (!visited[cx + px, cy + py])
                    {
                        visited[cx + px, cy + py] = true;
                        if (input[my][mx] != '#')
                            toGoNew.Enqueue((px, py));
                    }
                }
            }
            totals[times] = toGoNew.Count;
            if (times >= 2)
            {
                totals[times] += totals[times - 2];
                diff[times] = totals[times] - totals[times - 2];
                diff2[times] = diff[times] - diff[times - 2];
            }
            if (times > 500)
            {
                periodDetection.Update(diff2[times], a => (period: times - a.lastOccurance, lastOccurance: times, count: (times - a.lastOccurance == a.period) ? ++a.count : 1));
                period = periodDetection.Values.FirstOrDefault(a => a.count >= 3).period;
                if (period != 0)
                    break;
            }
            toGo = toGoNew;
        }
        Advent.AssertAnswer1(totals[Advent.UseSampleData ? 6 : 64], expected: 3600, sampleExpected: 16);

        if (period % 2 == 1)
            period *= 2;
        if ((times + totalTimes) % 2 == 1)
            times--;
        times -= period;
        long answer2 = totals[times - 2];
        for (; times <= totalTimes; times += 2)
        {
            diff[times] = (diff[times - period] * 2 - diff[times - period * 2]);
            answer2 += diff[times];
        }
        Advent.AssertAnswer2(answer2, expected: 599763113936220, sampleExpected: 16733044);
    }
}