using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace AdventOfCode2022;

public class Day24 : IAdvent
{
    public record V2(int X, int Y)
    {
        public static V2 operator -(V2 a, V2 b) => new V2(a.X - b.X, a.Y - b.Y);
        public static V2 operator +(V2 a, V2 b) => new V2(a.X + b.X, a.Y + b.Y);
    }
    static char[] Facing = new[] { '>', 'v', '<', '^' };
    static V2[] MoveD = new V2[] { new V2(1, 0), new V2(0, 1), new V2(-1, 0), new V2(0, -1), new V2(0, 0) };

    public void Run()
    {
        string[] input = Advent.ReadInputLines();
        var field = input
            .Select((line, y) => line.Select((c, x) => (p: new V2(x, y), c)))
            .SelectMany(a => a)
            .Where(p => p.c == '#')
            .ToDictionary(p => p.p, p => p.c);
        (V2 p, char c)[] wind = input
            .Select((line, y) => line.Select((c, x) => (p: new V2(x, y), c)))
            .SelectMany(a => a)
            .Where(p => p.c != '.' && p.c != '#')
            .Select(p => (p.p, p.c))
            .ToArray();
        int minX = field.Select(m => m.Key.X).Min();
        int maxX = field.Select(m => m.Key.X).Max();
        int minY = field.Select(m => m.Key.Y).Min();
        int maxY = field.Select(m => m.Key.Y).Max();

        Dictionary<V2, char>[] allWindPos = GetAllWindPositions();
        V2 entrance = new V2(1, 0);
        V2 exit = new V2(maxX - 1, maxY);
        int iteration = 0;
        
        DoIt(entrance, exit);
        Advent.AssertAnswer1(iteration, expected: 322, sampleExpected: 18);


        DoIt(exit, entrance);
        DoIt(entrance, exit);
        Advent.AssertAnswer2(iteration, expected: 974, sampleExpected: 54);


        (V2 p, char c)[] IterateWind((V2 p, char c)[] wind)
        {
            return wind.Select(w =>
            {
                int windIndex = Array.IndexOf(Facing, w.c);
                V2 np = w.p + MoveD[windIndex];
                if (np.X == minX) np = new V2(maxX - 1, np.Y);
                if (np.X == maxX) np = new V2(minX + 1, np.Y);
                if (np.Y == minY) np = new V2(np.X, maxY - 1);
                if (np.Y == maxY) np = new V2(np.X, minX + 1);
                return (np, w.c);
            }).ToArray();
        }

        int DoIt(V2 current, V2 goal)
        {
            var queue = new Queue<V2>();
            queue.Enqueue(current);

            while (true)
            {
                var windPos = allWindPos[(iteration + 1) % allWindPos.Length];
                var newQueue = new Queue<V2>();
                while (queue.Count > 0)
                {
                    var pos = queue.Dequeue();
                    if (pos == goal)
                        return iteration;

                    foreach (var m in MoveD)
                    {
                        var ne = pos + m;
                        if (ne.Y < 0)
                            continue;
                        if (ne.Y > maxY)
                            continue;

                        if (!windPos.ContainsKey(ne) && !field.ContainsKey(ne))
                            newQueue.Enqueue(ne);
                    }
                }
                queue = new Queue<V2>(newQueue
                    .Distinct()
                    .OrderByDescending(p => (goal == exit) ? (p.X + p.Y) : (999 - p.X - p.Y))
                    .Take(50));
                iteration += 1;
            }
        }

        Dictionary<V2, char>[] GetAllWindPositions()
        {
            int sX = (maxX - minX - 1);
            int sY = (maxY - minY - 1);
            var allWindPos = new Dictionary<V2, char>[sX * sY / (int)BigInteger.GreatestCommonDivisor(sX, sY)];
            for (int i = 0; i < allWindPos.Length; i++)
            {
                Dictionary<V2, char> dw = (
                        from w in wind
                        group w by w.p into g
                        let cnt = (char)('0' + g.Count())
                        select (p: g.Key, c: (g.Count() > 1 ? cnt : g.First().c))
                         ).ToDictionary(p => p.p, p => p.c);

                allWindPos[i] = dw;
                wind = IterateWind(wind);
            }

            return allWindPos;
        }
    }
}