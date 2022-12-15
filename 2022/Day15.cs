namespace AdventOfCode2022;

public class Day15 : IAdvent
{
    public void Run()
    {
        var input = Advent.ReadInputLines()
            .Select(line => Regex.Matches(line, @"-?\d+").Select(m => m.Value).Select(int.Parse).ToArray())
            .Select(m => (s: (x: m[0], y: m[1]), b: (x: m[2], y: m[3])))
            .ToArray();

        var rowY = Advent.UseSampleData ? 10 : 2000000;

        var (ranges, beaconCount) = DoIt(rowY);

        int answer1 = ranges.Sum(r => r.end - r.start) - beaconCount;
        Advent.AssertAnswer1(answer1, expected: 5166077, sampleExpected: 26);

        long answer2 = 0;
        for (int y = 0; y < 4000000; y++)
        {
            (ranges, _)  = DoIt(y);
            if (ranges.Count != 1)
            {
                int x = ranges[0].end;
                answer2 = 4000000L * x + y;
                break;
            }
        }
        Advent.AssertAnswer2(answer2, expected: 13071206703981, sampleExpected: 56000011);


        (List<(int start, int end)> mergedRanges, int beaconCount) DoIt(int rowY)
        {
            var ranges =
                from l in input
                let extent = Math.Abs(l.s.y - l.b.y) + Math.Abs(l.s.x - l.b.x) - Math.Abs(l.s.y - rowY)
                where extent > 0
                orderby l.s.x - extent, l.s.x + extent
                select (start: l.s.x - extent, end: l.s.x + extent + 1, l.b);

            var beacons = new HashSet<int>();
            foreach (var (start, end, b) in ranges)
            {
                if (b.y == rowY)
                    beacons.Add(b.x);
            }

            var mergedRanges = new List<(int start, int end)>();
            (int start, int end) cur = (int.MinValue, 0);
            foreach (var r in ranges)
            {
                if (cur.start == int.MinValue || r.start > cur.end)
                {
                    if (cur.start != int.MinValue)
                        mergedRanges.Add(cur);
                    cur = (r.start, r.end);
                }
                cur = (cur.start, Math.Max(cur.end, r.end));
            }
            mergedRanges.Add(cur);

            return (mergedRanges, beacons.Count);
        }
    }
}