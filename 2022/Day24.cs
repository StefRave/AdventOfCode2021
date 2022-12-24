using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        var allWindPos = GetAllWindPositions();

        int answer1 = DoIt(totalTrips: 1);
        Advent.AssertAnswer1(answer1, expected: 322, sampleExpected: 18);

        int answer2 = DoIt(totalTrips: 3);
        Advent.AssertAnswer2(answer2, expected: 974, sampleExpected: 54);


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

        int DoIt(int totalTrips = 1)
        {
            V2 entrance = new V2(1, 0);
            V2 exit = new V2(maxX - 1, maxY);

            var queue = new Queue<(V2 p, int trip, V2 goal)>();
            queue.Enqueue((entrance, 0, exit));

            for (int iteration = 0; iteration < 10000; iteration++)
            {
                var windPos = allWindPos[iteration % allWindPos.Length];
                var newQueue = new Queue<(V2 e, int t, V2 goal)>();
                while (queue.Count > 0)
                {
                    var (pos, trip, currentGoal) = queue.Dequeue();
                    if (pos == currentGoal)
                    {
                        trip++;
                        if (trip == totalTrips)
                            return iteration - 1;

                        currentGoal = currentGoal == entrance ? exit : entrance;
                    }

                    foreach (var m in MoveD)
                    {
                        var ne = pos + m;
                        if (ne.Y < 0)
                            continue;
                        if (ne.Y > maxY)
                            continue;

                        if (!windPos.ContainsKey(ne) && !field.ContainsKey(ne))
                            newQueue.Enqueue((ne, trip, currentGoal));
                    }
                }
                queue = new Queue<(V2, int, V2)>(newQueue
                    .Distinct()
                    .OrderByDescending(p => p.t * 1000 + ((p.goal == exit) ? (p.e.X + p.e.Y) : (999 - p.e.X - p.e.Y)))
                    .Take(50));
            }
            return -1;
        }

        Dictionary<V2, char>[] GetAllWindPositions()
        {
            var allWindPos = new Dictionary<V2, char>[(maxX - minX - 1) * (maxY - minY - 1)];
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