namespace AdventOfCode2022;

public class Day19 : IAdvent
{
    record M(int O, int C, int B, int G)
    {
        public static M operator +(M a, M b) => new(a.O + b.O, a.C + b.C, a.B + b.B, a.G + b.G);
        public static M operator -(M a, M b) => new(a.O - b.O, a.C - b.C, a.B - b.B, a.G - b.G);
        public static bool operator <=(M a, M b) => a.O <= b.O && a.C <= b.C && a.B <= b.B && a.G <= b.G;
        public static bool operator >=(M a, M b) => a.O >= b.O && a.C >= b.C && a.B >= b.B && a.G >= b.G;
    }
    static readonly M[] RobotType = new M[] { new (1, 0, 0, 0), new (0, 1, 0, 0), new (0, 0, 1, 0), new (0, 0, 0, 1) };

    public void Run()
    {
        // Blueprint 0: Each ore robot costs 1 ore.  Each clay robot costs 2 ore.  Each obsidian robot costs 3 ore and 4  clay.  Each geode robot costs 5 ore and 6 obsidian.
        var bluePrint = Advent.ReadInputLines()
            .Select(line =>
            {
                var m = Regex.Matches(line, @"\d+").Select(m => int.Parse(m.Value)).ToArray();
                return new M[] {
                    new M(m[1], 0, 0, 0),
                    new M(m[2], 0, 0, 0),
                    new M(m[3], m[4], 0, 0),
                    new M(m[5], 0, m[6], 0),
                };
            })
            .ToArray();

        long answer1 = 0;
        for (int i = 0; i < bluePrint.Length; i++)
        {
            int max = DoIt(bluePrint[i], 24);
            answer1 += max * (i + 1);
        }
        Advent.AssertAnswer1(answer1, expected: 1150, sampleExpected: 33);


        long answer2 = 1;
        for (int i = 0; i < Math.Min(bluePrint.Length, 3); i++)
        {
            int max = DoIt(bluePrint[i], 32);
            answer2 *= max;
        }
        Advent.AssertAnswer2(answer2, expected: 37367, sampleExpected: 3348);
    }

    private static int DoIt(M[] bluePrint, int totalMinutes)
    {
        var todo = new Queue<(M robots, M resources)>();
        todo.Enqueue((new M(O: 1, C: 0, B: 0, G: 0), new M(0, 0, 0, 0 )));

        for (int minute = 1; minute <= totalMinutes; minute++)
        {
            var todoNew = new Queue<(M robots, M resources)>();
            while (todo.Count > 0)
            {
                var (robots, resources) = todo.Dequeue();
                var newResources = resources + robots;

                for (int i = 0; i < bluePrint.Length; i++)
                    if (bluePrint[i] <= resources)
                        todoNew.Enqueue((robots + RobotType[i], newResources - bluePrint[i]));
                // Also add: do nothing
                todoNew.Enqueue((robots, newResources));
            }
            todo = new Queue<(M robots, M resources)>(todoNew
                .OrderByDescending(m => m.resources.G).ThenByDescending(m => m.robots.G)
                .ThenByDescending(m => m.resources.B).ThenByDescending(m => m.robots.B)
                .ThenByDescending(m => m.resources.C).ThenByDescending(m => m.robots.C)
                .Take(5000));
        }
        return todo.Select(m => m.resources.G).Max();
    }
}