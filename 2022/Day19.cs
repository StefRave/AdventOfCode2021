namespace AdventOfCode2022;

public class Day19 : IAdvent
{
    record M(int o, int c, int b, int g);
    
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

    private int DoIt(M[] bluePrint, int totalMinutes)
    {
        var todo = new Queue<(M robots, M resources)>();
        todo.Enqueue((new M(1, 0, 0, 0), new M(0, 0, 0, 0 )));

        for (int minute = 1; minute <= totalMinutes; minute++)
        {
            var todoNew = new Queue<(M robots, M resources)>();
            while (todo.Count > 0)
            {
                var (robots, resources) = todo.Dequeue();
                var newResources = new M(resources.o + robots.o, resources.c + robots.c, resources.b + robots.b, resources.g + robots.g);
                // do nothing
                todoNew.Enqueue((robots, newResources));
                for (int i = 0; i < bluePrint.Length; i++)
                {
                    var bp = bluePrint[i];
                    if (bp.o <= resources.o && bp.c <= resources.c && bp.b <= resources.b && bp.g <= resources.g)
                        todoNew.Enqueue(
                            (new M(robots.o + (i == 0 ? 1 : 0), robots.c + (i == 1 ? 1 : 0), robots.b + (i == 2 ? 1 : 0), robots.g + (i == 3 ? 1 : 0)),
                            new M(newResources.o - bp.o, newResources.c - bp.c, newResources.b - bp.b, newResources.g)));
                }
            }
            todo = new Queue<(M robots, M resources)>(todoNew.OrderByDescending(m => m.resources.g).ThenByDescending(m => m.robots.g)
                .ThenByDescending(m => m.resources.b).ThenByDescending(m => m.robots.b).ThenByDescending(m => m.robots.c).Take(5000));
        }
        int maxQ = todo.Select(m => m.resources.g).Max();
        return maxQ;
    }
}