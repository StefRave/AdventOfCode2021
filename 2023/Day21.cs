
using System;
using System.Linq;

namespace AdventOfCode2023;

public class Day21 : IAdvent
{
    public (int x, int y)[] deltas = [(-1, 0), (0, -1), (0, 1), (1, 0)];

    public void Run()
    {
        var input = Advent.ReadInput().SplitByNewLine();
        var (maxX, maxY) = (input[0].Length, input.Length);
        var map = new HashSet<(int x, int y)>();
        var start = (0, 0);
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] == 'S')
                    start = (x, y);
                else if (input[y ][x] != '.')
                    map.Add((x, y));
            }
        int totalTimes = Advent.UseSampleData ? 5000 : 26501365;
        long[] total = [1,0];
        long[] totals = new long[5000];
        long[] diff = new long[5000];
        long[] diff2 = new long[5000];
        var toGo = new Queue<(int x, int y)>([start]);
        
        var dict = new Dictionary<(int x, int y), char>();
        dict[start] = 'S';
        long answer2 = 0;
        for (int times = 1; times <= totalTimes; times++)
        {
            var toGoNew = new Queue<(int x, int y)>();
            while (toGo.TryDequeue(out var pos))
            {
                foreach (var (dx, dy) in deltas)
                {
                    var p = (x: pos.x + dx, y: pos.y + dy);
                    var pMod = ((p.x % maxX + maxX) % maxX, (p.y % maxY + maxX) % maxY);
                    if (!dict.ContainsKey(p))
                    {

                        bool obstructed = map.Contains(pMod);
                        dict.Add(p, obstructed ? '#' : 'o');
                        if (!obstructed)
                            toGoNew.Enqueue(p);
                    }
                }
            }
            totals[times] = total[times % 2] += toGoNew.Count;
            toGo = toGoNew;

            if (times == (Advent.UseSampleData ? 6 : 64))
            {
                Console.WriteLine("\r");
                Advent.AssertAnswer1(totals[times], expected: 3600, sampleExpected: 16);
            }
            if (times > 2)
            {
                diff[times] = totals[times] - totals[times - 2];
                diff2[times] = diff[times] - diff[times - 2];
                Console.Write($"\r{times} {diff2[times]}  {diff[times]}  {total[times % 2]}");
                if (times == 3000)
                {
                    Console.WriteLine("");
                    var ankor = diff2[2000..times]
                        .Select((n, i) => (n, i))
                        .GroupBy(n => n.n)
                        .OrderByDescending(g => g.Count())
                        .First()
                        .Select(gv => gv.i)
                        .ToArray();
                    var ankorDiff = ankor.Skip(1).Select((n, i) => ankor[i + 1] - ankor[i]).ToArray();
                    int period = ankorDiff[0] * 2;
                    int startAt = totalTimes - ((totalTimes - 2000) / period * period);

                    long t2 = totals[startAt];
                    for (int i = startAt + 2; i <= totalTimes; i += 2)
                    {
                        int index = (i - startAt - 2) % period + startAt + 2 - period;
                        var temp = diff[index];
                        diff[index] += (diff[index] - diff[index - period]);
                        diff[index - period] = temp;
                        t2 += diff[index];
                    }
                    answer2 = t2;
                    break;
                }
            }
        }
        Advent.AssertAnswer2(answer2, expected: 599763113936220, sampleExpected: 16733044);
    }
}
