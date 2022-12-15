namespace AdventOfCode2022;

public class Day15 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => Regex.Matches(line, @"-?\d+").Select(m => m.Value).Select(int.Parse).ToArray())
            .Select(m => (s: (x: m[0], y: m[1]), b: (x: m[2], y: m[3])))
            .OrderBy(m => m.s)
            .ToArray();

        var rowY = Advent.UseSampleData ? 10 : 2000000;
        var (impossiblePositions, xGap) = DoIt(rowY);
        int beaconCount = input.Where(l => l.b.y == rowY).Select(l => l.b.x).Distinct().Count();
        int answer1 = impossiblePositions - beaconCount;
        Advent.AssertAnswer1(answer1, expected: 5166077, sampleExpected: 26);

        long answer2 = 0;
        for (int y = 0; y < 4000000; y++)
        {
            (_, xGap) = DoIt(y);
            if (xGap.HasValue)
            {
                answer2 = 4000000L * xGap.Value + y;
                break;
            }
        }
        Advent.AssertAnswer2(answer2, expected: 13071206703981, sampleExpected: 56000011);


        (int impossiblePositions, int? xGap) DoIt(int rowY, bool sort = false)
        {
            if (sort)
                Array.Sort(input, (a, b) =>
                    (a.s.x - Math.Abs(a.s.y - a.b.y) - Math.Abs(a.s.x - a.b.x) + Math.Abs(a.s.y - rowY)) -
                    (b.s.x - Math.Abs(b.s.y - b.b.y) - Math.Abs(b.s.x - b.b.x) + Math.Abs(b.s.y - rowY)));

            (int start, int end) cur = (0,0);
            int prevStart = 0;
            bool first = true;
            foreach (var l in input)
            {
                var extent = Math.Abs(l.s.y - l.b.y) + Math.Abs(l.s.x - l.b.x) - Math.Abs(l.s.y - rowY);
                if (extent < 0)
                    continue;

                int start = l.s.x - extent;
                int end = l.s.x + extent + 1;
                if (first)
                {
                    cur = (start, end);
                    prevStart = start;
                    first = false;
                }
                prevStart = start;
                if (start > cur.end)
                {
                    if (!sort)
                        return DoIt(rowY, sort: true);

                    return (0, cur.end);
                }
                cur = (cur.start, Math.Max(cur.end, end));
            }
            return (cur.end - cur.start, null);
        }
    }
}